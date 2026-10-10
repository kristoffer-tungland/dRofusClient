using System.Text.Json;
using dRofusClient.Mcp;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace dRofusClient.Tests.Mcp;

public sealed class McpProtocolTests
{
    [Fact]
    public async Task StdioInitializesListsTwentyTwoToolsAndReturnsStructuredPreview()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await using var client = await ConnectAsync(false, null, timeout.Token);
        var tools = await client.ListToolsAsync(cancellationToken: timeout.Token);
        Assert.Equal(22, tools.Count);
        foreach (var name in new[] { "list_attribute_configurations", "get_attribute_configuration", "find_attribute_mappings" })
        {
            var tool = tools.Single(t => t.Name == name);
            Assert.True(tool.ProtocolTool.Annotations!.ReadOnlyHint);
            Assert.False(tool.JsonSchema.GetProperty("properties").TryGetProperty("server", out _));
            Assert.Contains("ToExternalApplication", tool.Description!);
            Assert.Contains("ToDrofus", tool.Description!);
            Assert.Contains("Key identifies", tool.Description!);
            Assert.Contains("write safeguards", tool.Description!);
            var invalid = await client.CallToolAsync(name, new Dictionary<string, object?>
            {
                ["id"] = 0, ["configurationId"] = 0, ["property"] = "Name", ["limit"] = 0
            }, cancellationToken: timeout.Token);
            Assert.True(invalid.IsError);
        }
        Assert.Contains("get_attribute_configuration", client.ServerInstructions!);
        Assert.Contains("find_attribute_mappings", client.ServerInstructions!);
        Assert.Contains("not proof values are synchronized", client.ServerInstructions!);
        var resolver = tools.Single(t => t.Name == "resolve_property");
        Assert.Contains("rooms", resolver.Description!);
        Assert.Contains("systems", resolver.Description!);
        Assert.Contains("entity", resolver.JsonSchema.GetRawText());
        var invalidResolution = await client.CallToolAsync("resolve_property",
            new Dictionary<string, object?> { ["entity"] = "unsupported", ["property"] = "Name" },
            cancellationToken: timeout.Token);
        Assert.True(invalidResolution.IsError);
        Assert.Contains(tools, t => t.Name == "search_custom_properties");
        Assert.Contains(tools, t => t.Name == "resolve_custom_property");
        foreach (var name in new[] { "search_custom_properties", "resolve_custom_property" })
        {
            var tool = tools.Single(t => t.Name == name);
            Assert.Contains("entity", tool.JsonSchema.GetRawText());
            Assert.Contains("propertyGroup", tool.JsonSchema.GetRawText());
            var invalid = await client.CallToolAsync(name, new Dictionary<string, object?>
            {
                ["entity"] = "unsupported", ["property"] = "Power"
            }, cancellationToken: timeout.Token);
            Assert.True(invalid.IsError);
        }
        Assert.Contains(tools, t => t.Name == "get_item_history");
        Assert.Contains(tools, t => t.Name == "get_occurrence_history");
        Assert.Contains(tools, t => t.Name == "create_item");
        foreach (var name in new[] { "search_items", "get_item", "search_occurrences", "get_occurrence",
            "get_item_history", "get_occurrence_history", "create_item", "update_item", "update_occurrence",
            "search_rooms", "get_room", "get_room_history", "get_room_occurrences", "create_room", "update_room" })
        {
            var description = tools.Single(t => t.Name == name).Description!;
            Assert.Contains("get_field_metadata", description);
            Assert.Contains("search_custom_properties", description);
            Assert.Contains("resolve_custom_property", description);
            Assert.Contains("resolve_property", description);
            Assert.Contains(name.Contains("occurrence") ? "entity='occurrences'" :
                name.Contains("room") ? "entity='rooms'" : "entity='items'", description);
            Assert.Contains("proactively", description, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("ambiguous", description);
            Assert.Contains("list_attribute_configurations", description);
            Assert.Contains("find_attribute_mappings", description);
        }
        Assert.Contains("in fields", tools.Single(t => t.Name == "get_item").Description!);
        Assert.Contains("in fields", tools.Single(t => t.Name == "get_occurrence").Description!);
        foreach (var name in new[] { "update_item", "update_occurrence", "update_room" })
        {
            var description = tools.Single(t => t.Name == name).Description!;
            Assert.Contains("changes keys", description);
            Assert.Contains("readOnly=false", description);
            Assert.Contains("true or null must not be written", description);
        }
        Assert.Contains("statuses, not changes", tools.Single(t => t.Name == "update_occurrence").Description!);
        Assert.Contains("accepts only the creation fields", tools.Single(t => t.Name == "create_item").Description!);
        Assert.Contains("update_item", tools.Single(t => t.Name == "create_item").Description!);
        Assert.Contains("separate preview/approval", tools.Single(t => t.Name == "create_item").Description!);
        foreach (var name in new[] { "get_item_history", "get_occurrence_history" })
            Assert.Contains("History filters target log fields, not entity property IDs", tools.Single(t => t.Name == name).Description!);
        Assert.Contains("get_field_metadata", client.ServerInstructions!);
        Assert.Contains("search_custom_properties", client.ServerInstructions!);
        Assert.Contains("resolve_custom_property", client.ServerInstructions!);
        Assert.Contains("first use resolve_property", client.ServerInstructions!);
        Assert.Contains("get_room_occurrences", client.ServerInstructions!);
        Assert.Contains("not proof of physical/BIM placement", client.ServerInstructions!);
        Assert.Contains("missing/null requirements are unknown", client.ServerInstructions!);
        Assert.Contains("not row count", client.ServerInstructions!);
        foreach (var name in new[] { "search_rooms", "get_room", "get_room_history", "get_room_occurrences" })
            Assert.True(tools.Single(t => t.Name == name).ProtocolTool.Annotations!.ReadOnlyHint);
        foreach (var name in new[] { "create_room", "update_room" })
        {
            var tool = tools.Single(t => t.Name == name);
            Assert.False(tool.ProtocolTool.Annotations!.ReadOnlyHint);
            Assert.True(tool.ProtocolTool.Annotations.DestructiveHint);
            Assert.False(tool.ProtocolTool.Annotations.IdempotentHint);
            Assert.Contains("DROFUS_ENABLE_WRITES=true", tool.Description!);
            Assert.True(tool.JsonSchema.GetProperty("properties").GetProperty("preview").GetProperty("default").GetBoolean());
        }
        Assert.DoesNotContain(tools, t => t.Name == "delete_room");
        Assert.Contains("update_room", client.ServerInstructions!);
        var roomPreview = await client.CallToolAsync("create_room", RoomCreationArguments(), cancellationToken: timeout.Token);
        Assert.NotEqual(true, roomPreview.IsError);
        Assert.Equal("preview", roomPreview.StructuredContent!.Value.GetProperty("outcome").GetString());
        var roomCreateRejected = await client.CallToolAsync("create_room", RoomCreationArguments(false), cancellationToken: timeout.Token);
        Assert.True(roomCreateRejected.IsError);
        Assert.Contains(roomCreateRejected.Content.OfType<TextContentBlock>(), block => block.Text.Contains("Writes are disabled"));
        var roomUpdateRejected = await client.CallToolAsync("update_room", new Dictionary<string, object?>
        {
            ["id"] = 8, ["changes"] = new Dictionary<string, object> { ["name"] = "Lab" }, ["preview"] = false
        }, cancellationToken: timeout.Token);
        Assert.True(roomUpdateRejected.IsError);
        Assert.Contains(roomUpdateRejected.Content.OfType<TextContentBlock>(), block => block.Text.Contains("Writes are disabled"));
        var invalidRoom = await client.CallToolAsync("get_room_occurrences",
            new Dictionary<string, object?> { ["roomId"] = 0 }, cancellationToken: timeout.Token);
        Assert.True(invalidRoom.IsError);
        var create = tools.Single(t => t.Name == "create_item");
        Assert.DoesNotContain("server", create.JsonSchema.GetRawText());
        var preview = await client.CallToolAsync("create_item", CreationArguments(), cancellationToken: timeout.Token);
        Assert.NotEqual(true, preview.IsError);
        Assert.Equal("preview", preview.StructuredContent!.Value.GetProperty("outcome").GetString());
        var rejected = await client.CallToolAsync("create_item", CreationArguments(preview: false), cancellationToken: timeout.Token);
        Assert.True(rejected.IsError);
        Assert.Contains(rejected.Content.OfType<TextContentBlock>(), block => block.Text.Contains("Writes are disabled"));
    }

    [Theory]
    [InlineData("create_item")]
    [InlineData("create_room")]
    public async Task EnabledWritesWithoutElicitationFailClosed(string tool)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await using var client = await ConnectAsync(true, null, timeout.Token);
        var result = await client.CallToolAsync(tool, tool == "create_item" ? CreationArguments(false) : RoomCreationArguments(false), cancellationToken: timeout.Token);
        Assert.True(result.IsError);
        Assert.Contains(result.Content.OfType<TextContentBlock>(), block => block.Text.Contains("interactive form elicitation"));
    }

    [Theory]
    [InlineData("create_item", "decline", false)]
    [InlineData("create_item", "cancel", false)]
    [InlineData("create_item", "accept", false)]
    [InlineData("create_room", "decline", false)]
    [InlineData("create_room", "cancel", false)]
    [InlineData("create_room", "accept", false)]
    public async Task HostMustExplicitlyApproveExactProposal(string tool, string action, bool approve)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        ElicitRequestParams? received = null;
        var options = new McpClientOptions
        {
            Capabilities = new() { Elicitation = new() { Form = new() } },
            Handlers = new()
            {
                ElicitationHandler = (request, token) =>
                {
                    received = request;
                    return ValueTask.FromResult(new ElicitResult
                    {
                        Action = action,
                        Content = new Dictionary<string, JsonElement> { ["approve"] = JsonSerializer.SerializeToElement(approve) }
                    });
                }
            }
        };
        await using var client = await ConnectAsync(true, options, timeout.Token);
        var result = await client.CallToolAsync(tool, tool == "create_item" ? CreationArguments(false) : RoomCreationArguments(false), cancellationToken: timeout.Token);
        Assert.NotNull(received);
        Assert.Contains(tool == "create_item" ? "Chair" : "Lab", received.Message);
        Assert.Contains("test_db", received.Message);
        Assert.DoesNotContain("test-password", received.Message);
        Assert.Equal("declined", result.StructuredContent!.Value.GetProperty("outcome").GetString());
    }

    private static Dictionary<string, object?> RoomCreationArguments(bool preview = true) => new()
    {
        ["fields"] = new Dictionary<string, object> { ["name"] = "Lab" },
        ["preview"] = preview
    };

    private static Dictionary<string, object?> CreationArguments(bool preview = true) => new()
    {
        ["fields"] = new Dictionary<string, object> { ["name"] = "Chair", ["level_id"] = 7 },
        ["preview"] = preview
    };

    private static Task<McpClient> ConnectAsync(bool writes, McpClientOptions? options, CancellationToken cancellationToken)
    {
        var testAssembly = typeof(McpProtocolTests).Assembly.Location;
        var transport = new StdioClientTransport(new()
        {
            Name = "dRofus MCP protocol test",
            Command = "dotnet",
            Arguments = ["exec", "--runtimeconfig", Path.ChangeExtension(testAssembly, ".runtimeconfig.json"),
                "--depsfile", Path.ChangeExtension(testAssembly, ".deps.json"), typeof(ReadTools).Assembly.Location],
            EnvironmentVariables = new Dictionary<string, string?>
            {
                ["DROFUS_BASE_URL"] = "https://api-no.drofus.com",
                ["DROFUS_DATABASE"] = "test_db",
                ["DROFUS_PROJECT_ID"] = "01",
                ["DROFUS_USERNAME"] = "test-user",
                ["DROFUS_PASSWORD"] = "test-password",
                ["DROFUS_ENABLE_WRITES"] = writes.ToString()
            }
        });
        return McpClient.CreateAsync(transport, options, cancellationToken: cancellationToken);
    }
}
