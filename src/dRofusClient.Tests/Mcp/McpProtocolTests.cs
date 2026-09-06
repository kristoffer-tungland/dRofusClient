using System.Text.Json;
using dRofusClient.Mcp;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace dRofusClient.Tests.Mcp;

public sealed class McpProtocolTests
{
    [Fact]
    public async Task StdioInitializesListsTenToolsAndReturnsStructuredPreview()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await using var client = await ConnectAsync(false, null, timeout.Token);
        var tools = await client.ListToolsAsync(cancellationToken: timeout.Token);
        Assert.Equal(10, tools.Count);
        Assert.Contains(tools, t => t.Name == "get_item_history");
        Assert.Contains(tools, t => t.Name == "get_occurrence_history");
        Assert.Contains(tools, t => t.Name == "create_item");
        var create = tools.Single(t => t.Name == "create_item");
        Assert.DoesNotContain("server", create.JsonSchema.GetRawText());
        var preview = await client.CallToolAsync("create_item", CreationArguments(), cancellationToken: timeout.Token);
        Assert.NotEqual(true, preview.IsError);
        Assert.Equal("preview", preview.StructuredContent!.Value.GetProperty("outcome").GetString());
        var rejected = await client.CallToolAsync("create_item", CreationArguments(preview: false), cancellationToken: timeout.Token);
        Assert.True(rejected.IsError);
        Assert.Contains(rejected.Content.OfType<TextContentBlock>(), block => block.Text.Contains("Writes are disabled"));
    }

    [Fact]
    public async Task EnabledWritesWithoutElicitationFailClosed()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await using var client = await ConnectAsync(true, null, timeout.Token);
        var result = await client.CallToolAsync("create_item", CreationArguments(preview: false), cancellationToken: timeout.Token);
        Assert.True(result.IsError);
        Assert.Contains(result.Content.OfType<TextContentBlock>(), block => block.Text.Contains("interactive form elicitation"));
    }

    [Theory]
    [InlineData("decline", false)]
    [InlineData("cancel", false)]
    [InlineData("accept", false)]
    public async Task HostMustExplicitlyApproveExactProposal(string action, bool approve)
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
        var result = await client.CallToolAsync("create_item", CreationArguments(preview: false), cancellationToken: timeout.Token);
        Assert.NotNull(received);
        Assert.Contains("Chair", received.Message);
        Assert.Contains("test_db", received.Message);
        Assert.DoesNotContain("test-password", received.Message);
        Assert.Equal("declined", result.StructuredContent!.Value.GetProperty("outcome").GetString());
    }

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
