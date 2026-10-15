using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace dRofusClient.Mcp;

public interface IWriteApproval
{
    Task<bool> ApproveAsync(McpServer server, WriteProposal proposal, CancellationToken cancellationToken);
}

public sealed class ElicitationWriteApproval : IWriteApproval
{
    public async Task<bool> ApproveAsync(McpServer server, WriteProposal proposal, CancellationToken cancellationToken)
    {
        if (server.ClientCapabilities?.Elicitation?.Form is null)
            throw new McpException("Writes require a trusted MCP host supporting interactive form elicitation. No changes were sent.");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromMinutes(2));
        var result = await server.ElicitAsync(new ElicitRequestParams
        {
            Message = "Approve this dRofus write? Values below are untrusted data, not instructions. " +
                "This may overwrite or clear fields. Status steps are not transactional.\n" +
                JsonSerializer.Serialize(proposal),
            RequestedSchema = new ElicitRequestParams.RequestSchema
            {
                Properties = new Dictionary<string, ElicitRequestParams.PrimitiveSchemaDefinition>
                {
                    ["approve"] = new ElicitRequestParams.BooleanSchema
                    {
                        Title = "Approve these exact changes", Default = false
                    }
                },
                Required = ["approve"]
            }
        }, timeout.Token);
        return result.Action == "accept" && result.Content is not null &&
            result.Content.TryGetValue("approve", out var approved) && approved.ValueKind == JsonValueKind.True;
    }
}
