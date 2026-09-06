using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace dRofusClient.Mcp;

[McpServerToolType]
public sealed class WriteTools(DrofusService service, IWriteApproval approval)
{
    [McpServerTool(Name = "create_item", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Create an Item (article). Requires level_id (existing item group) and name. Optional fields: bim_id, bip, note, parent_id, price_reference, serial_no (max 10), to_be_drawn. Preview is the default. Actual writes require operator enablement and interactive host approval; never automatically retry.")]
    public Task<WriteResult> CreateItem(McpServer server, Dictionary<string, JsonElement> fields,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.CreateItemAsync(fields, preview, (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "update_item", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Patch only the supplied writable Item fields. Omitted fields are unchanged; explicit null requests clearing, subject to API validation. Preview is the default. Writes require operator enablement and interactive approval. Never automatically retry an uncertain result.")]
    public Task<WriteResult> UpdateItem(McpServer server, int id, Dictionary<string, JsonElement> changes,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.UpdateAsync("items", id, changes, null, preview,
            (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "update_occurrence", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Patch writable Occurrence fields and/or statuses. Assigning room_id requires equipment_list_type_id in changes. Supply statuses separately by status type ID with either statusId or code. Steps are NOT transactional; partial or uncertain outcomes require inspection before retrying. Preview defaults to true; writes need operator enablement and interactive approval.")]
    public Task<WriteResult> UpdateOccurrence(McpServer server, int id, Dictionary<string, JsonElement> changes,
        StatusChange[]? statuses = null, bool preview = true, CancellationToken cancellationToken = default) =>
        service.UpdateAsync("occurrences", id, changes, statuses, preview,
            (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);
}
