using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace dRofusClient.Mcp;

[McpServerToolType]
public sealed class WriteTools(DrofusService service, IWriteApproval approval)
{
    private const string ResolutionGuidance =
        " First call resolve_property for the target entity to resolve exact API names and verified built-in aliases/synonyms before custom labels. " +
        "Never replace a built-in match with keyword search. Ambiguous results and fuzzy suggestions require user clarification; only a resolvedId is confirmed.";
    [McpServerTool(Name = "create_item", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Create an Item (article). Requires level_id (existing item group) and name. Optional fields: bim_id, bip, note, parent_id, price_reference, serial_no (max 10), to_be_drawn. Preview is the default. Actual writes require operator enablement and interactive host approval; never automatically retry. " +
        "Proactively inspect get_field_metadata (entity='items'); search_custom_properties and resolve_custom_property are custom-only fallbacks after built-in lookup fails. Never guess IDs and clarify ambiguous matches. This tool accepts only the creation fields listed above, even if metadata exposes other writable properties. Set supported additional properties afterward with update_item using the created ID, verified writable field IDs/types, and a separate preview/approval." + ResolutionGuidance)]
    public Task<WriteResult> CreateItem(McpServer server, Dictionary<string, JsonElement> fields,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.CreateItemAsync(fields, preview, (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "update_item", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Patch only the supplied writable Item fields. Omitted fields are unchanged; explicit null requests clearing, subject to API validation. Preview is the default. Writes require operator enablement and interactive approval. Never automatically retry an uncertain result. " +
        "Before setting user-named properties, proactively inspect get_field_metadata (entity='items'); search_custom_properties and resolve_custom_property are custom-only fallbacks after built-in lookup fails. Use returned IDs verbatim as changes keys and validate types/units. Never guess IDs; clarify ambiguous matches. Only readOnly=false is eligible for writes; true or null must not be written." + ResolutionGuidance)]
    public Task<WriteResult> UpdateItem(McpServer server, int id, Dictionary<string, JsonElement> changes,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.UpdateAsync("items", id, changes, null, preview,
            (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "create_room", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Create a Room. Requires a non-empty name (max 500 characters). Optional fields: architect_no, description, designed_area, drawing_name, drawing_no, note, programmed_area, room_function_id, user_room_no. " +
        "Preview defaults to true. Actual writes require DROFUS_ENABLE_WRITES=true and interactive host approval; never automatically retry creation. " +
        "Proactively inspect get_field_metadata (entity='rooms'); search_custom_properties and resolve_custom_property are custom-only fallbacks after built-in lookup fails. Never guess IDs; clarify ambiguous matches. " +
        "This tool accepts only the creation fields listed above. Set additional verified writable requirements afterward with update_room and a separate preview/approval." + ResolutionGuidance)]
    public Task<WriteResult> CreateRoom(McpServer server, Dictionary<string, JsonElement> fields,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.CreateRoomAsync(fields, preview, (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "update_room", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Patch only supplied writable Room fields, including verified project requirement properties. Omitted fields are unchanged; explicit null requests clearing, subject to API validation. " +
        "Preview defaults to true. Actual writes require DROFUS_ENABLE_WRITES=true and interactive host approval. Selected values are checked again after approval; never automatically retry an uncertain result. " +
        "Before setting user-named properties, proactively inspect get_field_metadata (entity='rooms'); search_custom_properties and resolve_custom_property are custom-only fallbacks after built-in lookup fails. " +
        "Use returned IDs verbatim as changes keys and validate types/units. Never guess IDs; clarify ambiguous matches. Only readOnly=false is eligible for writes; true or null must not be written. " +
        "Changing a room requirement does not change assigned occurrences or prove BIM placement. Occurrence assignments/statuses use update_occurrence, not this tool." + ResolutionGuidance)]
    public Task<WriteResult> UpdateRoom(McpServer server, int id, Dictionary<string, JsonElement> changes,
        bool preview = true, CancellationToken cancellationToken = default) =>
        service.UpdateAsync("rooms", id, changes, null, preview,
            (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);

    [McpServerTool(Name = "update_occurrence", ReadOnly = false, Destructive = true, Idempotent = false, UseStructuredContent = true)]
    [Description("Patch writable Occurrence fields and/or statuses. Assigning room_id requires equipment_list_type_id in changes. Supply statuses separately by status type ID with either statusId or code. Steps are NOT transactional; partial or uncertain outcomes require inspection before retrying. Preview defaults to true; writes need operator enablement and interactive approval. " +
        "Before setting user-named properties, proactively inspect get_field_metadata (entity='occurrences'); search_custom_properties and resolve_custom_property are custom-only fallbacks after built-in lookup fails. Use returned IDs verbatim as changes keys and validate types/units. Never guess IDs; clarify ambiguous matches. Only readOnly=false is eligible for field writes; true or null must not be written. Dynamic occurrence status fields must use statuses, not changes." + ResolutionGuidance)]
    public Task<WriteResult> UpdateOccurrence(McpServer server, int id, Dictionary<string, JsonElement> changes,
        StatusChange[]? statuses = null, bool preview = true, CancellationToken cancellationToken = default) =>
        service.UpdateAsync("occurrences", id, changes, statuses, preview,
            (proposal, token) => approval.ApproveAsync(server, proposal, token), cancellationToken);
}
