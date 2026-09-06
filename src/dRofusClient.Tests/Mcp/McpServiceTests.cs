using System.Net;
using System.Text;
using System.Text.Json;
using dRofusClient.Mcp;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol;

namespace dRofusClient.Tests.Mcp;

public sealed class McpServiceTests
{
    [Fact]
    public async Task SearchIsBoundedAndEscapesFilterLiterals()
    {
        using var fixture = new Fixture("""[{"id":1},{"id":2},{"id":3}]""");
        var result = await fixture.Service.SearchAsync("items",
            [new("name", "contains", Json("\"O'Brien' or id gt 0\""))], null, 2, 4, default);
        Assert.True(result.HasMore);
        Assert.Equal(6, result.NextOffset);
        var request = Assert.Single(fixture.Handler.Requests);
        Assert.Contains("top=3", request.Uri);
        Assert.Contains("skip=4", request.Uri);
        Assert.Contains("contains(name,'O''Brien'' or id gt 0')", Uri.UnescapeDataString(request.Uri));
        Assert.Equal(2, JsonSerializer.SerializeToElement(result.Data).GetArrayLength());
    }

    [Theory]
    [InlineData("A&B")]
    [InlineData("Room #1")]
    [InlineData("%27 or id gt 0 or name eq %27")]
    [InlineData("x&$top=9999")]
    [InlineData("A+B")]
    public async Task FilterValuesCannotChangeHttpQuery(string value)
    {
        using var fixture = new Fixture("[]");
        await fixture.Service.SearchAsync("items", [new("name", "eq", JsonSerializer.SerializeToElement(value))],
            null, 25, 0, default);
        var uri = new Uri(Assert.Single(fixture.Handler.Requests).Uri);
        Assert.Empty(uri.Fragment);
        var parameters = uri.Query.TrimStart('?').Split('&')
            .Select(part => part.Split('=', 2)).ToDictionary(parts => parts[0], parts => Uri.UnescapeDataString(parts[1]));
        Assert.Equal(5, parameters.Count);
        Assert.Equal("26", parameters["$top"]);
        Assert.Equal($"name eq '{value.Replace("'", "''")}'", parameters["$filter"]);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(101, 0)]
    [InlineData(1, -1)]
    [InlineData(1, int.MaxValue)]
    public async Task InvalidPaginationNeverCallsApi(int limit, int offset)
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.SearchAsync("items", null, null, limit, offset, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Theory]
    [InlineData("id) or true", "eq", "1")]
    [InlineData("id", "invalid", "1")]
    [InlineData("name", "contains", "42")]
    [InlineData("name", "eq", "{}")]
    public async Task InvalidFiltersNeverCallApi(string field, string op, string value)
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.SearchAsync("items",
            [new(field, op, Json(value))], null, 25, 0, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task HistoryUsesCorrectEndpointAndUtcBounds()
    {
        using var fixture = new Fixture("""[{"occurrence_id":8,"old_value":"old","new_value":"new","note":"data"}]""");
        var result = await fixture.Service.HistoryAsync("occurrences", 8,
            DateTimeOffset.Parse("2026-01-01T12:00:00+02:00"), DateTimeOffset.Parse("2026-01-02T12:00:00Z"),
            [new("username", "eq", Json("\"user\""))], 25, 0, default);
        var request = Assert.Single(fixture.Handler.Requests);
        Assert.Contains("/occurrences/8/logs", request.Uri);
        var query = Uri.UnescapeDataString(request.Uri);
        Assert.Contains("2026-01-01T10:00:00.0000000Z", query);
        Assert.Contains("orderby=time", query);
        Assert.Contains("old_value", query);
        Assert.Contains("API change log only", result.Notice);
    }

    [Fact]
    public async Task MetadataCombinesSchemaAndProjectFieldsConservatively()
    {
        using var fixture = new Fixture("""
            [{"id":"custom_field","name":"Custom","dataType":"string"},
             {"id":"editable_field","name":"Editable","dataType":"integer","readOnly":false},
             {"id":"id","name":"ID","dataType":"integer","readOnly":false},
             {"id":"name","name":"Name","dataType":"string","readOnly":true},
             {"id":"created_by","name":"Creator","dataType":"string","readOnly":false}]
            """);
        var result = await fixture.Service.MetadataAsync("items", 100, 0, default);
        var fields = Assert.IsType<FieldDefinition[]>(result.Data);
        Assert.Null(fields.Single(f => f.Id == "custom_field").ReadOnly);
        Assert.False(fields.Single(f => f.Id == "editable_field").ReadOnly);
        Assert.True(fields.Single(f => f.Id == "id").ReadOnly);
        Assert.True(fields.Single(f => f.Id == "name").ReadOnly);
        Assert.True(fields.Single(f => f.Id == "created_by").ReadOnly);
    }

    [Fact]
    public async Task CreatePreviewMakesNoRequestsAndNeedsNoWriteEnablement()
    {
        using var fixture = new Fixture();
        var result = await fixture.Service.CreateItemAsync(Fields("""{"name":"Chair","level_id":7}"""), true,
            (_, _) => throw new Exception("Approval must not run"), default);
        Assert.Equal("preview", result.Outcome);
        Assert.Empty(fixture.Handler.Requests);
    }

    [Theory]
    [InlineData("""{"name":"Chair"}""")]
    [InlineData("""{"name":"","level_id":7}""")]
    [InlineData("""{"name":"Chair","level_id":0}""")]
    [InlineData("""{"name":"Chair","level_id":null}""")]
    [InlineData("""{"name":"Chair","level_id":7,"serial_no":"12345678901"}""")]
    [InlineData("""{"name":"Chair","level_id":7,"id":42}""")]
    public async Task InvalidCreationIsRejected(string json)
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.CreateItemAsync(Fields(json), true, Approve, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task ReadOnlyModeBlocksCreateAndUpdateBeforeApiCalls()
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.CreateItemAsync(
            Fields("""{"name":"Chair","level_id":7}"""), false, Approve, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.UpdateAsync(
            "items", 1, Fields("""{"name":"Chair"}"""), null, false, Approve, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task DeclinedCreationMakesNoRequests()
    {
        using var fixture = new Fixture(writes: true);
        var result = await fixture.Service.CreateItemAsync(Fields("""{"name":"Chair","level_id":7}"""),
            false, (_, _) => Task.FromResult(false), default);
        Assert.Equal("declined", result.Outcome);
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task ApprovedCreationUsesTypedContractAndReadsBack()
    {
        using var fixture = new Fixture(true, """{"id":42}""", """{"id":42,"name":"Chair"}""");
        var result = await fixture.Service.CreateItemAsync(Fields("""{"name":"Chair","level_id":7,"bim_id":"B-1"}"""),
            false, Approve, default);
        Assert.Equal("applied", result.Outcome);
        Assert.Equal(42, result.Id);
        Assert.Equal(HttpMethod.Post, fixture.Handler.Requests[0].Method);
        Assert.Contains("\"level_id\":7", fixture.Handler.Requests[0].Body);
        Assert.Equal(HttpMethod.Get, fixture.Handler.Requests[1].Method);
    }

    [Fact]
    public async Task SparseUpdatePreservesExplicitNullAndNoUnrequestedFields()
    {
        using var fixture = new Fixture(true, "[]", """{"id":1,"note":"old"}""",
            """{"id":1,"note":"old"}""", """{"id":1}""", """{"id":1}""");
        var result = await fixture.Service.UpdateAsync("items", 1, Fields("""{"note":null}"""), null, false, Approve, default);
        Assert.Equal("applied", result.Outcome);
        var patch = Assert.Single(fixture.Handler.Requests.Where(r => r.Method == HttpMethod.Patch));
        Assert.Equal("""{"note":null}""", patch.Body);
        Assert.Equal("application/merge-patch+json", patch.ContentType);
    }

    [Theory]
    [InlineData("""{"id":3}""")]
    [InlineData("""{"number":"x"}""")]
    [InlineData("""{"bim_key":"x"}""")]
    [InlineData("""{"unknown":"x"}""")]
    [InlineData("""{"price":"wrong type"}""")]
    public async Task UnsafeItemUpdatesAreRejected(string changes)
    {
        using var fixture = new Fixture(true, "[]");
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.UpdateAsync("items", 1, Fields(changes),
            null, false, Approve, default));
        Assert.DoesNotContain(fixture.Handler.Requests, r => r.Method == HttpMethod.Patch);
    }

    [Fact]
    public async Task RoomAssignmentRequiresSchedule()
    {
        using var fixture = new Fixture(true, "[]");
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.UpdateAsync("occurrences", 1,
            Fields("""{"room_id":5}"""), null, false, Approve, default));
        Assert.DoesNotContain(fixture.Handler.Requests, r => r.Method == HttpMethod.Patch);
    }

    [Fact]
    public async Task ChangeDuringApprovalReturnsConflictWithoutPatch()
    {
        using var fixture = new Fixture(true, "[]", """{"id":1,"name":"before"}""", """{"id":1,"name":"changed"}""");
        var result = await fixture.Service.UpdateAsync("items", 1, Fields("""{"name":"new"}"""), null, false, Approve, default);
        Assert.Equal("conflict", result.Outcome);
        Assert.DoesNotContain(fixture.Handler.Requests, r => r.Method == HttpMethod.Patch);
    }

    [Fact]
    public async Task DeclinedUpdateDoesNotPatch()
    {
        using var fixture = new Fixture(true, "[]", """{"id":1,"name":"before"}""");
        var result = await fixture.Service.UpdateAsync("items", 1, Fields("""{"name":"new"}"""), null,
            false, (_, _) => Task.FromResult(false), default);
        Assert.Equal("declined", result.Outcome);
        Assert.DoesNotContain(fixture.Handler.Requests, r => r.Method == HttpMethod.Patch);
    }

    [Theory]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task FailureAfterFieldsReportsPartialAndStopsLaterStatusSteps(HttpStatusCode statusCode)
    {
        const string metadata = """
            [{"id":"ce1_id_or_parents","name":"Status","dataType":"integer"},
             {"id":"ce2_id","name":"Status2","dataType":"integer"}]
            """;
        using var fixture = new Fixture(true, metadata, """{"id":1,"quantity":1}""", """{"id":1,"quantity":1}""",
            """{"id":1,"quantity":2}""");
        fixture.Handler.Responses.Enqueue(new(statusCode) { Content = new StringContent("sensitive upstream response") });
        var result = await fixture.Service.UpdateAsync("occurrences", 1, Fields("""{"quantity":2}"""),
            [new(1, StatusId: 5), new(2, Code: "A")], false, Approve, default);
        Assert.Equal("partial", result.Outcome);
        Assert.Equal(["fields"], result.CompletedSteps);
        Assert.EndsWith("/occurrences/1/statuses/1", fixture.Handler.Requests[^1].Uri);
        Assert.DoesNotContain("sensitive", JsonSerializer.Serialize(result));
    }

    [Theory]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task SuccessfulWriteWithFailedReadBackIsNotReportedAsRetryableFailure(HttpStatusCode statusCode)
    {
        using var fixture = new Fixture(true, """{"id":42}""");
        fixture.Handler.Responses.Enqueue(new(statusCode));
        var result = await fixture.Service.CreateItemAsync(Fields("""{"name":"Chair","level_id":7}"""), false, Approve, default);
        Assert.Equal("applied_unverified", result.Outcome);
        Assert.Equal(42, result.Id);
        Assert.Single(fixture.Handler.Requests.Where(r => r.Method == HttpMethod.Post));
    }

    [Fact]
    public async Task UpstreamErrorsAreRedacted()
    {
        using var fixture = new Fixture();
        fixture.Handler.Responses.Enqueue(new(HttpStatusCode.Forbidden) { Content = new StringContent("private server message") });
        var exception = await Assert.ThrowsAsync<McpException>(() => fixture.Service.GetAsync("items", 1, null, default));
        Assert.Contains("403", exception.Message);
        Assert.DoesNotContain("private", exception.Message);
    }

    private static Task<bool> Approve(WriteProposal _, CancellationToken __) => Task.FromResult(true);
    private static JsonElement Json(string value) => JsonSerializer.Deserialize<JsonElement>(value);
    private static Dictionary<string, JsonElement> Fields(string value) =>
        JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(value)!;

    private sealed class Fixture : IDisposable
    {
        public RecordingHandler Handler { get; } = new();
        public DrofusService Service { get; }
        private readonly HttpClient _http;

        public Fixture(params string[] responses) : this(false, responses) { }

        public Fixture(bool writes, params string[] responses)
        {
            foreach (var response in responses)
                Handler.Responses.Enqueue(new(HttpStatusCode.OK)
                {
                    Content = new StringContent(response, Encoding.UTF8, "application/json")
                });
            _http = new HttpClient(Handler);
            var client = new dRofusClient(_http, new NonePromptHandler());
            client.Setup(dRofusConnectionArgs.CreateNoServer("test_db", "01", "test-user", "test-password"));
            Service = new(client, new ServerSettings
            {
                BaseUrl = "https://api-no.drofus.com", Database = "test_db", ProjectId = "01",
                Username = "test-user", Password = "test-password", EnableWrites = writes
            }, new FieldCatalog(), NullLogger<DrofusService>.Instance);
        }

        public void Dispose() { Service.Dispose(); _http.Dispose(); }
    }

    private sealed record RecordedRequest(HttpMethod Method, string Uri, string? Body, string? ContentType);

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<RecordedRequest> Requests { get; } = [];
        public Queue<HttpResponseMessage> Responses { get; } = new();
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(new(request.Method, request.RequestUri!.ToString(),
                request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken),
                request.Content?.Headers.ContentType?.MediaType));
            return Responses.Dequeue();
        }
    }
}
