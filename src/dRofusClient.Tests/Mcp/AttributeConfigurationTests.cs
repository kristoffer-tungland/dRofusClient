using System.Net;
using System.Text.Json;
using dRofusClient.Mcp;
using ModelContextProtocol;

namespace dRofusClient.Tests.Mcp;

public sealed partial class McpServiceTests
{
    private const string MappingConfiguration = """
        [{"id":7,"name":"Room mapping","config_type":"room","applicable_to":["Revit"],
          "available_to_users":true,"is_default":true,"elements":[
            {"id":1,"configuration":7,"drofus_attribute_id":"Room.ID","drofus_attribute_label":"Identity",
             "external_attribute_id":"{PARAM-GUID}","external_attribute_label":"dRofus ID","direction":"Key"},
            {"id":2,"configuration":7,"drofus_attribute_id":"Room.Name","drofus_attribute_label":"Name",
             "external_attribute_id":"BuiltIn:ROOM_NAME","external_attribute_label":"Room name","direction":"ToExternalApplication"},
            {"id":3,"configuration":7,"drofus_attribute_id":"Custom:1","drofus_attribute_label":"Power",
             "external_attribute_id":"instance:Power","external_attribute_label":"Power","direction":"ToDrofus"},
            {"id":4,"configuration":7,"drofus_attribute_id":"Custom:2","drofus_attribute_label":"Power",
             "external_attribute_id":"type:Power","external_attribute_label":"Power","direction":null},
            {"id":5,"configuration":7,"drofus_attribute_id":"Custom:3","drofus_attribute_label":"Room.Name",
             "external_attribute_id":"other","external_attribute_label":"Other","direction":"FutureDirection"},
            {"id":6,"configuration":7,"drofus_attribute_id":"Custom:4"}]}]
        """;

    [Fact]
    public async Task ConfigurationListIsBoundedAndPreservesContextWithoutElements()
    {
        using var fixture = new Fixture("""
            [{"id":7,"name":"Room mapping","config_type":"room","applicable_to":["Revit"],
              "available_to_users":true,"is_default":true,"elements":[{"direction":"FutureDirection"}]},
             {"id":8,"name":"Other","config_type":"space","is_default":true}]
            """);
        var result = await fixture.Service.ListAttributeConfigurationsAsync(
            [new("config_type", "eq", Json("\"room\"")), new("name", "contains", Json("\"A&B's\""))], 1, 3, default);
        var rows = JsonSerializer.SerializeToElement(result.Data);
        Assert.Equal(1, rows.GetArrayLength());
        Assert.Equal("Revit", rows[0].GetProperty("applicable_to")[0].GetString());
        Assert.True(rows[0].GetProperty("is_default").GetBoolean());
        Assert.True(rows[0].GetProperty("available_to_users").GetBoolean());
        Assert.False(rows[0].TryGetProperty("elements", out _));
        Assert.True(result.HasMore);
        Assert.Equal(4, result.NextOffset);
        Assert.Equal("test_db", result.Project.Database);
        var request = Assert.Single(fixture.Handler.Requests);
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Contains("/attributeconfigurations?", request.Uri);
        var parameters = new Uri(request.Uri).Query.TrimStart('?').Split('&').Select(p => p.Split('=', 2))
            .ToDictionary(p => p[0], p => Uri.UnescapeDataString(p[1]));
        Assert.Equal("2", parameters["$top"]);
        Assert.Equal("3", parameters["$skip"]);
        Assert.Equal("id asc", parameters["$orderby"]);
        Assert.Contains("config_type eq 'room'", parameters["$filter"]);
        Assert.Contains("contains(name,'A&B''s')", parameters["$filter"]);
        Assert.DoesNotContain("elements", parameters["$select"]);
    }

    [Fact]
    public async Task ConfigurationReadPagesMappingsAndPreservesRawIdentifiersAndDirections()
    {
        using var fixture = new Fixture(MappingConfiguration, MappingConfiguration);
        var first = await fixture.Service.GetAttributeConfigurationAsync(7, 3, 0, default);
        var data = Assert.IsType<AttributeMappingResult>(first.Data);
        Assert.Equal("available", data.Status);
        Assert.Equal(6, data.MatchCount);
        Assert.False(data.Configuration.ContainsKey("elements"));
        Assert.Equal("room", data.Configuration["config_type"].GetString());
        Assert.Equal("Room.ID", data.Mappings[0].GetProperty("drofus_attribute_id").GetString());
        Assert.Equal("{PARAM-GUID}", data.Mappings[0].GetProperty("external_attribute_id").GetString());
        Assert.Equal(["Key", "ToExternalApplication", "ToDrofus"],
            data.Mappings.Select(m => m.GetProperty("direction").GetString()).ToArray());
        Assert.True(first.HasMore);
        Assert.Equal(3, first.NextOffset);
        var last = await fixture.Service.GetAttributeConfigurationAsync(7, 3, first.NextOffset!.Value, default);
        var tail = Assert.IsType<AttributeMappingResult>(last.Data);
        Assert.Equal(JsonValueKind.Null, tail.Mappings[0].GetProperty("direction").ValueKind);
        Assert.Equal("FutureDirection", tail.Mappings[1].GetProperty("direction").GetString());
        Assert.False(tail.Mappings[2].TryGetProperty("direction", out _));
        Assert.False(last.HasMore);
        Assert.Null(last.NextOffset);
        Assert.Contains("Missing, null or unfamiliar directions are unknown", last.Notice);
        Assert.Contains("not proof values are synchronized", last.Notice);
        Assert.All(fixture.Handler.Requests, request =>
        {
            var uri = Uri.UnescapeDataString(request.Uri);
            Assert.Contains("$top=2", uri);
            Assert.Contains("$skip=0", uri);
            Assert.Contains("$filter=id eq 7", uri);
            Assert.Contains("elements", uri);
        });
    }

    [Theory]
    [InlineData("Room.Name", "drofus", "matched", 1, 2)]
    [InlineData("Room.Name", "either", "matched", 1, 2)]
    [InlineData("room.name", "drofus", "matched", 1, 5)]
    [InlineData("room name", "external", "matched", 1, 2)]
    [InlineData("BuiltIn:ROOM_NAME", "external", "matched", 1, 2)]
    [InlineData("BuiltIn:ROOM_NAME", "drofus", "not_found", 0, 0)]
    [InlineData("builtin:room_name", "external", "not_found", 0, 0)]
    [InlineData("Power", "either", "ambiguous", 2, 3)]
    [InlineData("Pow", "either", "not_found", 0, 0)]
    [InlineData("not a field & $top=999", "either", "not_found", 0, 0)]
    public async Task MappingLookupUsesExactIdsBeforeLabelsWithoutGuessing(string property, string side,
        string status, int count, int firstId)
    {
        using var fixture = new Fixture(MappingConfiguration);
        var result = await fixture.Service.FindAttributeMappingsAsync(7, property, side, 25, 0, default);
        var data = Assert.IsType<AttributeMappingResult>(result.Data);
        Assert.Equal(status, data.Status);
        Assert.Equal(count, data.MatchCount);
        Assert.Equal(count, data.Mappings.Count);
        if (count > 0)
            Assert.Equal(firstId, data.Mappings[0].GetProperty("id").GetInt32());
        Assert.DoesNotContain(Uri.EscapeDataString(property), Assert.Single(fixture.Handler.Requests).Uri);
    }

    [Fact]
    public async Task MappingAmbiguityIncludesHiddenAndDuplicateIdMatches()
    {
        using var fixture = new Fixture(MappingConfiguration, MappingConfiguration,
            MappingConfiguration.Replace("\"Custom:2\"", "\"Custom:1\""));
        var first = await fixture.Service.FindAttributeMappingsAsync(7, "Power", "drofus", 1, 0, default);
        Assert.Equal("ambiguous", Assert.IsType<AttributeMappingResult>(first.Data).Status);
        Assert.Equal(2, Assert.IsType<AttributeMappingResult>(first.Data).MatchCount);
        Assert.Single(Assert.IsType<AttributeMappingResult>(first.Data).Mappings);
        Assert.True(first.HasMore);
        var last = await fixture.Service.FindAttributeMappingsAsync(7, "Power", "drofus", 1, first.NextOffset!.Value, default);
        Assert.Equal("ambiguous", Assert.IsType<AttributeMappingResult>(last.Data).Status);
        Assert.False(last.HasMore);
        var duplicate = await fixture.Service.FindAttributeMappingsAsync(7, "Custom:1", "drofus", 1, 0, default);
        Assert.Equal("ambiguous", Assert.IsType<AttributeMappingResult>(duplicate.Data).Status);
        Assert.Equal(2, Assert.IsType<AttributeMappingResult>(duplicate.Data).MatchCount);
    }

    [Theory]
    [InlineData("""[{"id":7}]""", "unavailable", null)]
    [InlineData("""[{"id":7,"elements":null}]""", "unavailable", null)]
    [InlineData("""[{"id":7,"elements":[]}]""", "not_found", 0)]
    public async Task MappingLookupDistinguishesMissingElementsFromEmpty(string response, string status, int? count)
    {
        using var fixture = new Fixture(response);
        var result = await fixture.Service.FindAttributeMappingsAsync(7, "Name", "either", 25, 0, default);
        var data = Assert.IsType<AttributeMappingResult>(result.Data);
        Assert.Equal(status, data.Status);
        Assert.Equal(count, data.MatchCount);
        Assert.Empty(data.Mappings);
        Assert.False(result.HasMore);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("""[{"id":8}]""")]
    [InlineData("""[{"id":7},{"id":7}]""")]
    [InlineData("""[{"id":7,"elements":{}}]""")]
    [InlineData("""[{"id":7,"elements":[null]}]""")]
    [InlineData("""[{"name":"Missing ID"}]""")]
    public async Task ConfigurationReadRejectsMissingAmbiguousOrMalformedResponses(string response)
    {
        using var fixture = new Fixture(response);
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.GetAttributeConfigurationAsync(7, 25, 0, default));
        Assert.Single(fixture.Handler.Requests);
    }

    [Fact]
    public async Task MappingToolsRejectInvalidInputsBeforeApiCalls()
    {
        using var fixture = new Fixture();
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ListAttributeConfigurationsAsync(null, 101, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.ListAttributeConfigurationsAsync(
            [new("bad&field", "eq", Json("1"))], 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.GetAttributeConfigurationAsync(0, 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.GetAttributeConfigurationAsync(7, 0, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.GetAttributeConfigurationAsync(7, 25, -1, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.GetAttributeConfigurationAsync(7, 25, int.MaxValue, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.FindAttributeMappingsAsync(7, " ", "either", 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.FindAttributeMappingsAsync(7, new string('x', 1025), "either", 25, 0, default));
        await Assert.ThrowsAsync<McpException>(() => fixture.Service.FindAttributeMappingsAsync(7, "Name", "invalid", 25, 0, default));
        Assert.Empty(fixture.Handler.Requests);
    }

    [Fact]
    public async Task ConfigurationListDoesNotFollowUpstreamLinksAndHandlesEmptyPages()
    {
        using var fixture = new Fixture();
        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") };
        response.Headers.TryAddWithoutValidation("Link", "<https://example.invalid/next>; rel=\"next\"");
        fixture.Handler.Responses.Enqueue(response);
        var result = await fixture.Service.ListAttributeConfigurationsAsync(null, 25, 0, default);
        Assert.Equal(0, JsonSerializer.SerializeToElement(result.Data).GetArrayLength());
        Assert.False(result.HasMore);
        Assert.Single(fixture.Handler.Requests);
    }

    [Theory]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task AttributeConfigurationFailuresAreRedacted(HttpStatusCode status)
    {
        using var fixture = new Fixture();
        fixture.Handler.Responses.Enqueue(new(status) { Content = new StringContent("private mapping information") });
        var error = await Assert.ThrowsAsync<McpException>(() => fixture.Service.GetAttributeConfigurationAsync(7, 25, 0, default));
        Assert.DoesNotContain("private", error.Message);
    }
}
