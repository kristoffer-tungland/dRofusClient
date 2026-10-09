using System.Net;
using System.Text;
using System.Text.Json;
using dRofusClient.Mcp;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol;

namespace dRofusClient.Tests.Mcp;

public sealed class McpServiceTests
{
    [Theory]
    [InlineData("items", "price", "price")]
    [InlineData("occurrences", "Item ID", "article_id")]
    [InlineData("rooms", "RoomNumber", "architect_no")]
    [InlineData("systems", "SystemComponentId", "base_occurrence_id")]
    public async Task VerifiedResolutionFetchesEndpointMetadata(string entity, string property, string expected)
    {
        using var fixture = new Fixture("[]");
        var result = await fixture.Service.ResolvePropertyAsync(entity, property, 25, 0, default);
        Assert.Equal(expected, Assert.IsType<VerifiedPropertyResolution>(result.Data).ResolvedId);
        Assert.Equal("test_db", result.Project.Database);
        var request = Assert.Single(fixture.Handler.Requests);
        Assert.Equal(HttpMethod.Options, request.Method);
        Assert.EndsWith("/" + entity, request.Uri);
    }

    [Fact]
    public async Task VerifiedResolutionPagesAmbiguityAndRejectsInvalidInput()
    {
        using var fixture = new Fixture("[]");
        var result = await fixture.Service.ResolvePropertyAsync("items", "Serial Number", 1, 0, default);
        Assert.Equal("ambiguous", Assert.IsType<VerifiedPropertyResolution>(result.Data).Status);
        Assert.True(result.HasMore);
        Assert.Equal(1, result.NextOffset);
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ResolvePropertyAsync("other", "Name", 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ResolvePropertyAsync("items", " ", 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ResolvePropertyAsync("items", new string('a', 257), 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ResolvePropertyAsync("items", "name", 101, 0, default));
        Assert.Single(fixture.Handler.Requests);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ResolvingSystemsDoesNotEnableSystemWrites(bool preview)
    {
        using var fixture = new Fixture(writes: true);
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.UpdateAsync("systems", 1,
            Fields("""{"name":"Changed"}"""), null, preview, Approve, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task RoomPreviewsDoNotApproveOrWriteWithWritesDisabled()
    {
        using var fixture = new Fixture("[]", """{"id":8,"name":"Before"}""");
        var create = await fixture.Service.CreateRoomAsync(Fields("""{"name":"Lab"}"""), true,
            (_, _) => throw new Exception("Approval must not run"), default);
        Assert.Equal("preview", create.Outcome);
        Assert.Empty(fixture.Handler.Requests);
        var update = await fixture.Service.UpdateAsync("rooms", 8, Fields("""{"name":"Lab"}"""), null, true,
            (_, _) => throw new Exception("Approval must not run"), default);
        Assert.Equal("preview", update.Outcome);
        Assert.All(fixture.Handler.Requests, r => Assert.True(r.Method == HttpMethod.Options || r.Method == HttpMethod.Get));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("""{"name":null}""")]
    [InlineData("""{"name":" "}""")]
    [InlineData("""{"name":42}""")]
    [InlineData("""{"name":"Lab","room_function_id":0}""")]
    [InlineData("""{"name":"Lab","id":8}""")]
    [InlineData("""{"name":"Lab","fixture_sockets":6}""")]
    [InlineData("""{"name":"Lab","designed_area":"large"}""")]
    public async Task InvalidRoomCreationIsRejectedBeforeRequests(string fields)
    {
        using var fixture = new Fixture(writes: true);
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.CreateRoomAsync(Fields(fields), false, Approve, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task RoomCreationEnforcesNameLength()
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.CreateRoomAsync(
            new() { ["name"] = JsonSerializer.SerializeToElement(new string('a', 501)) }, true, Approve, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task ApprovedRoomCreationPreservesAllSupportedSchemaFields()
    {
        const string fields = """
            {"name":"Lab","architect_no":"A1","description":"Test","designed_area":12.5,"drawing_name":"Laboratory",
             "drawing_no":"D1","note":"Example","programmed_area":14,"room_function_id":3,"user_room_no":"U1"}
            """;
        using var fixture = new Fixture(true, """{"id":8}""", """{"id":8,"name":"Lab"}""");
        var approved = false;
        var result = await fixture.Service.CreateRoomAsync(Fields(fields), false, (proposal, _) =>
        {
            Assert.Equal("create_room", proposal.Operation);
            Assert.Equal(10, proposal.Changes.Count);
            approved = true;
            return Task.FromResult(true);
        }, default);
        Assert.True(approved);
        Assert.Equal("applied", result.Outcome);
        Assert.Equal(8, result.Id);
        var post = fixture.Handler.Requests[0];
        Assert.Equal(HttpMethod.Post, post.Method);
        Assert.EndsWith("/rooms", post.Uri);
        var body = Fields(post.Body!);
        Assert.Equal(10, body.Count);
        foreach (var (key, value) in Fields(fields))
            Assert.Equal(value.ToString(), body[key].ToString());
        Assert.Contains("/rooms/8?", fixture.Handler.Requests[1].Uri);
        Assert.Contains("designed_area", fixture.Handler.Requests[1].Uri);
    }

    [Fact]
    public async Task DeclinedRoomCreationMakesNoRequests()
    {
        using var fixture = new Fixture(writes: true);
        var result = await fixture.Service.CreateRoomAsync(Fields("""{"name":"Lab"}"""), false,
            (_, _) => Task.FromResult(false), default);
        Assert.Equal("declined", result.Outcome);
        Assert.Empty(fixture.Handler.Requests);
    }

    [Theory]
    [InlineData("""{"id":9}""")]
    [InlineData("""{"created":"2026-01-01"}""")]
    [InlineData("""{"deleted":true}""")]
    [InlineData("""{"full_name":"Lab"}""")]
    [InlineData("""{"room_func_no":"1"}""")]
    [InlineData("""{"unknown":1}""")]
    [InlineData("""{"fixture_unverified":1}""")]
    [InlineData("""{"fixture_readonly":1}""")]
    [InlineData("""{"ceiling_height":"tall"}""")]
    public async Task UnsafeRoomUpdatesAreRejected(string changes)
    {
        using var fixture = new Fixture(true, """
            [{"id":"fixture_unverified","name":"Unknown permission","dataType":"integer"},
             {"id":"fixture_readonly","name":"Read only","dataType":"integer","readOnly":true},
             {"id":"id","name":"ID","dataType":"integer","readOnly":false}]
            """);
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.UpdateAsync(
            "rooms", 8, Fields(changes), null, false, Approve, default));
        Assert.Equal(HttpMethod.Options, Assert.Single(fixture.Handler.Requests).Method);
    }

    [Fact]
    public async Task RoomUpdatesRejectOccurrenceStatuses()
    {
        using var fixture = new Fixture(writes: true);
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.UpdateAsync(
            "rooms", 8, Fields("""{"name":"Lab"}"""), [new(1, StatusId: 2)], false, Approve, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task RoomUpdatePreservesCustomRequirementsAndCeilingHeight()
    {
        const string changes = """{"ceiling_height":2.7,"fixture_sockets":6,"description":null}""";
        const string before = """{"id":8,"ceiling_height":2.5,"fixture_sockets":4,"description":"Old"}""";
        using var fixture = new Fixture(true,
            """[{"id":"fixture_sockets","name":"Sockets","dataType":"integer","readOnly":false}]""",
            before, before, """{"id":8}""", """{"id":8,"fixture_sockets":6}""");
        var result = await fixture.Service.UpdateAsync("rooms", 8, Fields(changes), null, false, (proposal, _) =>
        {
            Assert.Equal("update_rooms", proposal.Operation);
            Assert.Equal(4, JsonSerializer.SerializeToElement(proposal.Before).GetProperty("fixture_sockets").GetInt32());
            return Task.FromResult(true);
        }, default);
        Assert.Equal("applied", result.Outcome);
        var patch = Assert.Single(fixture.Handler.Requests.Where(r => r.Method == HttpMethod.Patch));
        Assert.EndsWith("/rooms/8", patch.Uri);
        Assert.Equal(changes, patch.Body);
        Assert.Equal("application/merge-patch+json", patch.ContentType);
        Assert.Contains("/rooms/8?", fixture.Handler.Requests[^1].Uri);
        Assert.DoesNotContain(fixture.Handler.Requests, r => r.Uri.Contains("/occurrences"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RoomCreationFailuresDoNotInviteRetries(bool readBackFailure)
    {
        using var fixture = readBackFailure ? new Fixture(true, """{"id":8}""") : new Fixture(writes: true);
        fixture.Handler.Responses.Enqueue(new(HttpStatusCode.Unauthorized) { Content = new StringContent("sensitive") });
        var result = await fixture.Service.CreateRoomAsync(Fields("""{"name":"Lab"}"""), false, Approve, default);
        Assert.Equal(readBackFailure ? "applied_unverified" : "uncertain", result.Outcome);
        Assert.Single(fixture.Handler.Requests.Where(r => r.Method == HttpMethod.Post));
        Assert.DoesNotContain("sensitive", JsonSerializer.Serialize(result));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RoomUpdateFailuresDoNotInviteRetries(bool readBackFailure)
    {
        using var fixture = new Fixture(true, "[]", """{"id":8,"name":"Old"}""", """{"id":8,"name":"Old"}""");
        if (readBackFailure)
            fixture.Handler.Responses.Enqueue(new(HttpStatusCode.OK) { Content = new StringContent("""{"id":8}""") });
        fixture.Handler.Responses.Enqueue(new(HttpStatusCode.Forbidden) { Content = new StringContent("sensitive") });
        var result = await fixture.Service.UpdateAsync("rooms", 8, Fields("""{"name":"Lab"}"""), null, false, Approve, default);
        Assert.Equal(readBackFailure ? "applied_unverified" : "uncertain", result.Outcome);
        Assert.Single(fixture.Handler.Requests.Where(r => r.Method == HttpMethod.Patch));
        Assert.DoesNotContain("sensitive", JsonSerializer.Serialize(result));
    }

    [Fact]
    public async Task RoomSearchSelectsIdentityAndPagesResults()
    {
        using var fixture = new Fixture("""[{"id":8,"architect_no":"A1","name":"Lab"},{"id":9}]""");
        var result = await fixture.Service.SearchAsync("rooms", [new("architect_no", "eq", Json("\"A1\""))],
            null, 1, 2, default);
        var request = Uri.UnescapeDataString(Assert.Single(fixture.Handler.Requests).Uri);
        Assert.Contains("/rooms?", request);
        Assert.Contains("architect_no eq 'A1'", request);
        Assert.Contains("$select=id,architect_no,name,room_function_id", request);
        Assert.Contains("$top=2", request);
        Assert.Contains("$skip=2", request);
        Assert.Equal(1, JsonSerializer.SerializeToElement(result.Data).GetArrayLength());
        Assert.True(result.HasMore);
        Assert.Equal(3, result.NextOffset);
    }

    [Fact]
    public async Task RoomRequirementDiscoveryAndReadUseProjectPropertyIds()
    {
        const string metadata = """
            [{"id":"architect_no","name":"Room number","dataType":"string"},
             {"id":"fixture_sockets","name":"Socket count","propertyGroup":"Electrical","dataType":"integer","readOnly":false},
             {"id":"fixture_water","name":"Water outlets","propertyGroup":"Plumbing","dataType":"boolean","readOnly":true}]
            """;
        using var fixture = new Fixture(metadata, metadata, metadata,
            """{"id":8,"fixture_sockets":6,"fixture_water":null}""");
        var fields = Assert.IsType<FieldDefinition[]>((await fixture.Service.MetadataAsync("rooms", 100, 0, default)).Data);
        Assert.Contains(fields, f => f.Id == "fixture_sockets" && f.Type == "integer");
        Assert.Contains(fields, f => f.Id == "fixture_water" && f.ReadOnly == true);
        var resolved = Assert.IsType<VerifiedPropertyResolution>((await fixture.Service.ResolvePropertyAsync(
            "rooms", "Socket count", 25, 0, default)).Data);
        Assert.Equal("fixture_sockets", resolved.ResolvedId);
        var custom = Assert.IsType<PropertyResolution>((await fixture.Service.ResolveCustomPropertyAsync(
            "rooms", "Water outlets", "Plumbing", 25, 0, default)).Data);
        Assert.Equal("fixture_water", custom.ResolvedId);
        var result = await fixture.Service.GetAsync("rooms", 8, ["id", resolved.ResolvedId!, custom.ResolvedId!], default);
        var data = JsonSerializer.SerializeToElement(result.Data);
        Assert.Equal(6, data.GetProperty("fixture_sockets").GetInt32());
        Assert.Equal(JsonValueKind.Null, data.GetProperty("fixture_water").ValueKind);
        Assert.Contains("/rooms/8?", fixture.Handler.Requests[^1].Uri);
        Assert.Contains("$select=id,fixture_sockets,fixture_water", Uri.UnescapeDataString(fixture.Handler.Requests[^1].Uri));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(3)]
    public async Task RoomOccurrencesEnforceRoomScopeAndPreserveQuantitiesAndSchedules(int? schedule)
    {
        using var fixture = new Fixture("""{"id":8}""", """
            [{"id":1,"article_id":10,"room_id":8,"equipment_list_type_id":3,"quantity":4},
             {"id":2,"article_id":11,"room_id":8,"equipment_list_type_id":3,"quantity":null},
             {"id":3,"article_id":10,"room_id":8,"equipment_list_type_id":4,"quantity":2}]
            """);
        var result = await fixture.Service.RoomOccurrencesAsync(8, schedule,
            [new("room_id", "eq", Json("9")), new("occurrence_name", "eq", Json("\"A&B's\""))],
            ["ifc_guids"], 2, 5, default);
        Assert.Contains("/rooms/8?", fixture.Handler.Requests[0].Uri);
        var uri = new Uri(fixture.Handler.Requests[1].Uri);
        var parameters = uri.Query.TrimStart('?').Split('&').Select(p => p.Split('=', 2))
            .ToDictionary(p => p[0], p => Uri.UnescapeDataString(p[1]));
        Assert.EndsWith("/occurrences", uri.AbsolutePath);
        Assert.Equal("3", parameters["$top"]);
        Assert.Equal("5", parameters["$skip"]);
        Assert.Equal("id asc", parameters["$orderby"]);
        Assert.Contains("room_id eq 9", parameters["$filter"]);
        Assert.Contains(" and room_id eq 8", parameters["$filter"]);
        Assert.Contains("occurrence_name eq 'A&B''s'", parameters["$filter"]);
        Assert.Equal(schedule.HasValue, parameters["$filter"].Contains("equipment_list_type_id eq 3"));
        foreach (var field in new[] { "id", "article_id", "room_id", "equipment_list_type_id", "quantity", "occurrence_name", "ifc_guids" })
            Assert.Contains(field, parameters["$select"].Split(','));
        var rows = JsonSerializer.SerializeToElement(result.Data);
        Assert.Equal(2, rows.GetArrayLength());
        Assert.Equal(4, rows[0].GetProperty("quantity").GetInt32());
        Assert.Equal(JsonValueKind.Null, rows[1].GetProperty("quantity").ValueKind);
        Assert.True(result.HasMore);
        Assert.Equal(7, result.NextOffset);
        Assert.Contains("not evidence of physical/BIM placement", result.Notice);
        Assert.Contains("null quantity is unknown", result.Notice);
    }

    [Fact]
    public async Task RoomOccurrenceContinuationAndEmptyRoomAreBounded()
    {
        using var fixture = new Fixture("""{"id":8}""", """[{"id":1},{"id":2}]""",
            """{"id":8}""", """[{"id":2}]""", """{"id":9}""", "[]");
        var first = await fixture.Service.RoomOccurrencesAsync(8, null, null, null, 1, 0, default);
        var last = await fixture.Service.RoomOccurrencesAsync(8, null, null, null, 1, first.NextOffset!.Value, default);
        Assert.Equal(2, JsonSerializer.SerializeToElement(last.Data)[0].GetProperty("id").GetInt32());
        Assert.False(last.HasMore);
        Assert.Null(last.NextOffset);
        Assert.Contains("$skip=1", fixture.Handler.Requests[3].Uri);
        var empty = await fixture.Service.RoomOccurrencesAsync(9, null, null, null, 25, 0, default);
        Assert.Equal(0, JsonSerializer.SerializeToElement(empty.Data).GetArrayLength());
        Assert.False(empty.HasMore);
    }

    [Theory]
    [InlineData(0, null, 25, 0)]
    [InlineData(8, 0, 25, 0)]
    [InlineData(8, -1, 25, 0)]
    [InlineData(8, null, 101, 0)]
    [InlineData(8, null, 25, -1)]
    public async Task RoomOccurrencesValidateBeforeAnyRequest(int roomId, int? schedule, int limit, int offset)
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.RoomOccurrencesAsync(
            roomId, schedule, null, null, limit, offset, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task RoomOccurrencesRejectInvalidFiltersAndFieldsBeforeRequests()
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.RoomOccurrencesAsync(
            8, null, [new("room_id", "or", Json("9"))], null, 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.RoomOccurrencesAsync(
            8, null, null, ["bad&field"], 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.RoomOccurrencesAsync(
            8, null, null, [], 25, 0, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task MissingRoomDoesNotReturnMisleadingEmptyAssignments()
    {
        using var fixture = new Fixture();
        fixture.Handler.Responses.Enqueue(new(HttpStatusCode.NotFound) { Content = new StringContent("private room details") });
        var error = await Assert.ThrowsAsync<McpException>(() => fixture.Service.RoomOccurrencesAsync(
            8, null, null, null, 25, 0, default));
        Assert.DoesNotContain("private", error.Message);
        Assert.Contains("404", error.Message);
        Assert.Contains("/rooms/8?", Assert.Single(fixture.Handler.Requests).Uri);
    }

    private const string CustomMetadata = """
        [{"id":"name","name":"Standard name","dataType":"string"},
         {"id":"budget_group","name":"Standard DTO field","dataType":"string"},
         {"id":"custom_a","name":"Power","propertyGroup":"Electrical","dataType":"decimal","unit":"W","readOnly":false},
         {"id":"custom_b","name":"Power","propertyGroup":"Mechanical","dataType":"integer","readOnly":true},
         {"id":"custom_c","name":"Finish","propertyGroup":"General","dataType":"Choice"},
         {"id":"ce12_id","name":"Status","propertyGroup":"Workflow","dataType":"integer"},
         {"id":"bad&field","name":"Invalid ID","dataType":"string"}]
        """;

    [Theory]
    [InlineData("items", "budget_group")]
    [InlineData("occurrences", "occurrence_name")]
    [InlineData("rooms", "architect_no")]
    public async Task CustomPropertyDiscoveryUsesLiveMetadataAndExcludesStandardFields(string entity, string standardId)
    {
        using var fixture = new Fixture(CustomMetadata.Replace("budget_group", standardId).Replace("\"id\":\"name\"", "\"id\":\"id\""));
        var result = await fixture.Service.SearchCustomPropertiesAsync(entity, null, null, 100, 0, default);
        var fields = Assert.IsType<CustomPropertyDefinition[]>(result.Data);
        Assert.DoesNotContain(fields, f => f.Id == "id" || f.Id == standardId || f.Id == "bad&field");
        Assert.Contains(fields, f => f.Id == "ce12_id");
        var power = fields.Single(f => f.Id == "custom_a");
        Assert.Equal("Electrical", power.PropertyGroup);
        Assert.Equal("Electrical: Power", power.Title);
        Assert.Equal("number", power.Type);
        Assert.Equal("decimal", power.DataType);
        Assert.Equal("W", power.Unit);
        Assert.False(power.ReadOnly);
        Assert.True(fields.Single(f => f.Id == "custom_b").ReadOnly);
        Assert.Null(fields.Single(f => f.Id == "custom_c").ReadOnly);
        Assert.Equal("Choice", fields.Single(f => f.Id == "custom_c").DataType);
        Assert.Equal("test_db", result.Project.Database);
        var request = Assert.Single(fixture.Handler.Requests);
        Assert.Equal(HttpMethod.Options, request.Method);
        Assert.EndsWith("/" + entity, request.Uri);
    }

    [Fact]
    public async Task CustomPropertySearchFiltersBeforePagination()
    {
        using var fixture = new Fixture(CustomMetadata, CustomMetadata);
        var first = await fixture.Service.SearchCustomPropertiesAsync("items", " pOw ", null, 1, 0, default);
        Assert.Equal("custom_a", Assert.Single(Assert.IsType<CustomPropertyDefinition[]>(first.Data)).Id);
        Assert.True(first.HasMore);
        Assert.Equal(1, first.NextOffset);
        var second = await fixture.Service.SearchCustomPropertiesAsync("items", "pow", null, 1, 1, default);
        Assert.Equal("custom_b", Assert.Single(Assert.IsType<CustomPropertyDefinition[]>(second.Data)).Id);
        Assert.False(second.HasMore);
        Assert.Null(second.NextOffset);
    }

    [Theory]
    [InlineData("CUSTOM_", "electrical", "custom_a")]
    [InlineData("electrical", null, "custom_a")]
    [InlineData("finish", null, "custom_c")]
    [InlineData("Power", "Mechanic", null)]
    [InlineData("Power&$top=999", null, null)]
    public async Task CustomPropertySearchMatchesLocally(string search, string? group, string? expectedId)
    {
        using var fixture = new Fixture(CustomMetadata);
        var result = await fixture.Service.SearchCustomPropertiesAsync("items", search, group, 25, 0, default);
        var fields = Assert.IsType<CustomPropertyDefinition[]>(result.Data);
        if (expectedId is null)
            Assert.Empty(fields);
        else
            Assert.Equal(expectedId, Assert.Single(fields).Id);
        Assert.DoesNotContain("?", Assert.Single(fixture.Handler.Requests).Uri);
    }

    [Theory]
    [InlineData(" CUSTOM_A ", null, "resolved", "custom_a", 1)]
    [InlineData("Power", "electrical", "resolved", "custom_a", 1)]
    [InlineData("mechanical: power", null, "resolved", "custom_b", 1)]
    [InlineData("Power", null, "ambiguous", null, 2)]
    [InlineData("Pow", null, "not_found", null, 0)]
    [InlineData("custom_a", "Mechanical", "not_found", null, 0)]
    [InlineData("name", null, "not_found", null, 0)]
    public async Task CustomPropertyResolutionDoesNotGuess(string property, string? group,
        string status, string? resolvedId, int count)
    {
        using var fixture = new Fixture(CustomMetadata);
        var result = await fixture.Service.ResolveCustomPropertyAsync("items", property, group, 25, 0, default);
        var resolution = Assert.IsType<PropertyResolution>(result.Data);
        Assert.Equal(status, resolution.Status);
        Assert.Equal(resolvedId, resolution.ResolvedId);
        Assert.Equal(count, resolution.MatchCount);
    }

    [Fact]
    public async Task AmbiguityIsDeterminedBeforePaging()
    {
        using var fixture = new Fixture(CustomMetadata, CustomMetadata);
        var first = await fixture.Service.ResolveCustomPropertyAsync("items", "Power", null, 1, 0, default);
        var resolution = Assert.IsType<PropertyResolution>(first.Data);
        Assert.Equal("ambiguous", resolution.Status);
        Assert.Null(resolution.ResolvedId);
        Assert.Equal(2, resolution.MatchCount);
        Assert.Single(resolution.Candidates);
        Assert.True(first.HasMore);
        var second = await fixture.Service.ResolveCustomPropertyAsync("items", "Power", null, 1, first.NextOffset!.Value, default);
        Assert.Equal("custom_b", Assert.Single(Assert.IsType<PropertyResolution>(second.Data).Candidates).Id);
        Assert.False(second.HasMore);
    }

    [Fact]
    public async Task ResolutionPrefersExactIdOverAnotherPropertyLabel()
    {
        using var fixture = new Fixture("""
            [{"id":"custom_a","name":"Other","dataType":"string"},
             {"id":"custom_b","name":"custom_a","dataType":"string"}]
            """);
        var result = await fixture.Service.ResolveCustomPropertyAsync("items", "custom_a", null, 25, 0, default);
        Assert.Equal("custom_a", Assert.IsType<PropertyResolution>(result.Data).ResolvedId);
    }

    [Fact]
    public async Task CustomPropertyLookupRejectsInvalidArgumentsBeforeRequests()
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.SearchCustomPropertiesAsync("unsupported", null, null, 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.SearchCustomPropertiesAsync("items", null, null, 101, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.SearchCustomPropertiesAsync("items", new string('a', 257), null, 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ResolveCustomPropertyAsync("items", " ", null, 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ResolveCustomPropertyAsync("items", "Power", new string('a', 257), 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ResolveCustomPropertyAsync("items", "Power", null, 25, -1, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task CustomPropertyLookupHandlesEmptyMetadataAndRedactsErrors()
    {
        using var fixture = new Fixture("[]");
        var result = await fixture.Service.ResolveCustomPropertyAsync("occurrences", "Power", null, 25, 0, default);
        Assert.Equal("not_found", Assert.IsType<PropertyResolution>(result.Data).Status);
        fixture.Handler.Responses.Enqueue(new(HttpStatusCode.Forbidden) { Content = new StringContent("private metadata") });
        var exception = await Assert.ThrowsAsync<McpException>(() =>
            fixture.Service.SearchCustomPropertiesAsync("occurrences", null, null, 25, 0, default));
        Assert.DoesNotContain("private metadata", exception.Message);
    }

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

    [Theory]
    [InlineData("occurrences")]
    [InlineData("rooms")]
    public async Task HistoryUsesCorrectEndpointAndUtcBounds(string entity)
    {
        using var fixture = new Fixture("""[{"occurrence_id":8,"old_value":"old","new_value":"new","note":"data"}]""");
        var result = await fixture.Service.HistoryAsync(entity, 8,
            DateTimeOffset.Parse("2026-01-01T12:00:00+02:00"), DateTimeOffset.Parse("2026-01-02T12:00:00Z"),
            [new("username", "eq", Json("\"user\""))], 25, 0, default);
        var request = Assert.Single(fixture.Handler.Requests);
        Assert.Contains($"/{entity}/8/logs", request.Uri);
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
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.CreateRoomAsync(
            Fields("""{"name":"Lab"}"""), false, Approve, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.UpdateAsync(
            "rooms", 8, Fields("""{"name":"Lab"}"""), null, false, Approve, default));
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

    [Theory]
    [InlineData("items")]
    [InlineData("rooms")]
    public async Task ChangeDuringApprovalReturnsConflictWithoutPatch(string entity)
    {
        using var fixture = new Fixture(true, "[]", """{"id":1,"name":"before"}""", """{"id":1,"name":"changed"}""");
        var result = await fixture.Service.UpdateAsync(entity, 1, Fields("""{"name":"new"}"""), null, false, Approve, default);
        Assert.Equal("conflict", result.Outcome);
        Assert.DoesNotContain(fixture.Handler.Requests, r => r.Method == HttpMethod.Patch);
    }

    [Theory]
    [InlineData("items")]
    [InlineData("rooms")]
    public async Task DeclinedUpdateDoesNotPatch(string entity)
    {
        using var fixture = new Fixture(true, "[]", """{"id":1,"name":"before"}""");
        var result = await fixture.Service.UpdateAsync(entity, 1, Fields("""{"name":"new"}"""), null,
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
