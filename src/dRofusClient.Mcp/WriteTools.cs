using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace dRofusClient.Mcp;

[McpServerToolType]
public sealed class WriteTools(DrofusService service, IWriteApproval approval)
{
    [McpServerTool(Name = "create_item", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Create an Item (article). Requires level_id (existing item group) and name. Optional fields: bim_id, bip, note, parent_id, price_reference, serial_no (max 10), to_be_drawn. Preview is the default. Actual writes require operator enablement and interactive host approval; never automatically retry. " +
        "Proactively use get_field_metadata (entity='items'), search_custom_properties and resolve_custom_property to identify user-requested properties; never guess IDs and clarify ambiguous matches. This tool accepts only the creation fields listed above, even if metadata exposes other writable properties. Set supported additional properties afterward with update_item using the created ID, verified writable field IDs/types, and a separate preview/approval.")]
    public Task<WriteResult> CreateItem(McpServer server, Dictionary<string, JsonElement> fields,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.CreateItemAsync(fields, preview, (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "update_item", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Patch only the supplied writable Item fields. Omitted fields are unchanged; explicit null requests clearing, subject to API validation. Preview is the default. Writes require operator enablement and interactive approval. Never automatically retry an uncertain result. " +
        "Before setting user-named properties, proactively use get_field_metadata (entity='items') for standard/custom fields, search_custom_properties for partial labels or groups, and resolve_custom_property for exact custom names. Use returned IDs verbatim as changes keys and validate types/units. Never guess IDs; clarify ambiguous matches. Only readOnly=false is eligible for writes; true or null must not be written.")]
    public Task<WriteResult> UpdateItem(McpServer server, int id, Dictionary<string, JsonElement> changes,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.UpdateAsync("items", id, changes, null, preview,
            (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "update_occurrence", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Patch writable Occurrence fields and/or statuses. Assigning room_id requires equipment_list_type_id in changes. Supply statuses separately by status type ID with either statusId or code. Steps are NOT transactional; partial or uncertain outcomes require inspection before retrying. Preview defaults to true; writes need operator enablement and interactive approval. " +
        "Before setting user-named properties, proactively use get_field_metadata (entity='occurrences') for standard/custom fields, search_custom_properties for partial labels or groups, and resolve_custom_property for exact custom names. Use returned IDs verbatim as changes keys and validate types/units. Never guess IDs; clarify ambiguous matches. Only readOnly=false is eligible for field writes; true or null must not be written. Dynamic occurrence status fields must use statuses, not changes.")]
    public Task<WriteResult> UpdateOccurrence(McpServer server, int id, Dictionary<string, JsonElement> changes,
        StatusChange[]? statuses = null, bool preview = true, CancellationToken cancellationToken = default) =>
        service.UpdateAsync("occurrences", id, changes, statuses, preview,
            (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);
}
