using System.ComponentModel;
using ModelContextProtocol.Server;

namespace dRofusClient.Mcp;

[McpServerToolType]
public sealed class ReadTools(DrofusService service)
{
    private const string ResolutionGuidance =
        " First proactively call resolve_property for the target entity: exact API names and verified built-in aliases/synonyms take precedence over custom labels. " +
        "Do not substitute keyword search for built-in resolution. Fuzzy results are suggestions only; ask the user to clarify ambiguous or unconfirmed matches.";

    [McpServerTool(Name = "search_rooms", ReadOnly = true, UseStructuredContent = true)]
    [Description("Find rooms by name, room number (architect_no), or verified properties. Filters are ANDed; read all pages and clarify ambiguous room identities before choosing an ID. " +
        "Use entity='rooms' with get_field_metadata; search_custom_properties and resolve_custom_property are custom-only fallbacks. " +
        "Select requested properties in fields. For sockets or water outlet requirements, resolve actual project properties; never invent requirement fields. " +
        "Then use get_room and get_room_occurrences to compare requirements with assigned equipment." + ResolutionGuidance)]
    public Task<ReadResult> SearchRooms(FieldFilter[]? filters = null, string[]? fields = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchAsync("rooms", filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_room", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read a room by positive ID. Default fields are a compact identity summary, not all room requirements. " +
        "Use entity='rooms' with get_field_metadata; search_custom_properties and resolve_custom_property are custom-only fallbacks. " +
        "Include resolved requirements in fields to answer questions such as socket quantity or water outlets. Missing/null values mean unknown, not zero or false. " +
        "Use get_room_occurrences to compare against dRofus assignments; room requirements and assigned equipment are different sources." + ResolutionGuidance)]
    public Task<ReadResult> GetRoom(int id, string[]? fields = null, CancellationToken cancellationToken = default) =>
        service.GetAsync("rooms", id, fields, cancellationToken);

    [McpServerTool(Name = "get_room_occurrences", ReadOnly = true, UseStructuredContent = true)]
    [Description("List occurrences assigned to a verified roomId, with mandatory room_id filtering. Optionally scope equipmentListTypeId to a known room schedule; filters only narrow the result. " +
        "Includes id, article_id, room_id, equipment_list_type_id, quantity and occurrence_name even when fields is supplied. " +
        "Read every page before totals; sum quantity rather than counting rows, treating null quantities as unknown and schedules separately. " +
        "Identify equipment using verified Item data via get_item/article_id; do not infer sockets or water outlets solely from an occurrence label. " +
        "Compare with resolved requirements from get_room. dRofus assignment is not proof of physical/BIM placement; actual placement checks require model evidence. " +
        "For additional occurrence fields use entity='occurrences', get_field_metadata and only after built-in lookup fails search_custom_properties or resolve_custom_property." + ResolutionGuidance)]
    public Task<ReadResult> GetRoomOccurrences(int roomId, int? equipmentListTypeId = null,
        FieldFilter[]? filters = null, string[]? fields = null, int limit = 25, int offset = 0,
        CancellationToken cancellationToken = default) =>
        service.RoomOccurrencesAsync(roomId, equipmentListTypeId, filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_room_history", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read the room's available API change log, oldest first, with inclusive date bounds and paginated log filters. " +
        "Use entity='rooms' with get_field_metadata; search_custom_properties and resolve_custom_property are custom-only fallbacks for identifying user-named properties. " +
        "History filters target log fields, not entity property IDs; inspect log field values before filtering a requirement's history." + ResolutionGuidance)]
    public Task<ReadResult> GetRoomHistory(int id, DateTimeOffset? from = null, DateTimeOffset? to = null,
        FieldFilter[]? filters = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.HistoryAsync("rooms", id, from, to, filters, limit, offset, cancellationToken);

    [McpServerTool(Name = "search_items", ReadOnly = true, UseStructuredContent = true)]
    [Description("Find Items (articles) in the configured project. Filters are ANDed. Returns a bounded page; use nextOffset while hasMore is true. " +
        "For user-requested properties, use entity='items'. Inspect get_field_metadata for standard/custom fields; only after built-in resolution fails use search_custom_properties or resolve_custom_property. Use returned IDs verbatim in fields and filter.field; include requested properties in fields. Never guess IDs; clarify ambiguous matches." + ResolutionGuidance)]
    public Task<ReadResult> SearchItems(FieldFilter[]? filters = null, string[]? fields = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchAsync("items", filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_item", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read an Item (article) by positive ID. Specify API field identifiers for extra fields; default is a compact summary. " +
        "For user-requested properties, use entity='items'. Inspect get_field_metadata for standard/custom fields; only after built-in resolution fails use search_custom_properties or resolve_custom_property. Include the returned IDs verbatim in fields; otherwise requested properties may be absent. Never guess IDs; clarify ambiguous matches." + ResolutionGuidance)]
    public Task<ReadResult> GetItem(int id, string[]? fields = null, CancellationToken cancellationToken = default) =>
        service.GetAsync("items", id, fields, cancellationToken);

    [McpServerTool(Name = "search_occurrences", ReadOnly = true, UseStructuredContent = true)]
    [Description("Find Occurrences, for example using article_id or room_id filters. Filters are ANDed. Returns a bounded page. " +
        "For user-requested properties, use entity='occurrences'. Inspect get_field_metadata for standard/custom fields; only after built-in resolution fails use search_custom_properties or resolve_custom_property. Use returned IDs verbatim in fields and filter.field; include requested properties in fields. Never guess IDs; clarify ambiguous matches." + ResolutionGuidance)]
    public Task<ReadResult> SearchOccurrences(FieldFilter[]? filters = null, string[]? fields = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchAsync("occurrences", filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_occurrence", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read an Occurrence by positive ID. An occurrence references an Item via article_id. Specify fields for additional data. " +
        "For user-requested properties, use entity='occurrences'. Inspect get_field_metadata for standard/custom fields; only after built-in resolution fails use search_custom_properties or resolve_custom_property. Include the returned IDs verbatim in fields; otherwise requested properties may be absent. Never guess IDs; clarify ambiguous matches." + ResolutionGuidance)]
    public Task<ReadResult> GetOccurrence(int id, string[]? fields = null, CancellationToken cancellationToken = default) =>
        service.GetAsync("occurrences", id, fields, cancellationToken);

    [McpServerTool(Name = "get_item_history", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read the Item's available API change log, oldest first, including old/new values, user, time, action and notes. Not a complete historical snapshot. " +
        "To identify a user-named property, consult get_field_metadata with entity='items'; search_custom_properties and resolve_custom_property are custom-only fallbacks. Clarify ambiguous matches. History filters target log fields, not entity property IDs; inspect returned log entries to determine how a property is represented in the log's field value." + ResolutionGuidance)]
    public Task<ReadResult> GetItemHistory(int id, DateTimeOffset? from = null, DateTimeOffset? to = null,
        FieldFilter[]? filters = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.HistoryAsync("items", id, from, to, filters, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_occurrence_history", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read the Occurrence's available API change log, oldest first. Date bounds are inclusive; filters use log fields such as username, action or field. " +
        "To identify a user-named property, consult get_field_metadata with entity='occurrences'; search_custom_properties and resolve_custom_property are custom-only fallbacks. Clarify ambiguous matches. History filters target log fields, not entity property IDs; inspect returned log entries to determine how a property is represented in the log's field value." + ResolutionGuidance)]
    public Task<ReadResult> GetOccurrenceHistory(int id, DateTimeOffset? from = null, DateTimeOffset? to = null,
        FieldFilter[]? filters = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.HistoryAsync("occurrences", id, from, to, filters, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_field_metadata", ReadOnly = true, UseStructuredContent = true)]
    [Description("Discover field IDs, labels, types, units and known readOnly restrictions for entity 'items', 'occurrences' or 'rooms'. Room changes use update_room with the same write enablement and approval as other writes. readOnly=null cannot be written. Use resolve_property to map a user's label to a verified built-in field before considering custom matches.")]
    public Task<ReadResult> GetFieldMetadata(string entity, int limit = 100, int offset = 0,
        CancellationToken cancellationToken = default) =>
        service.MetadataAsync(entity, limit, offset, cancellationToken);

    [McpServerTool(Name = "search_custom_properties", ReadOnly = true, UseStructuredContent = true)]
    [Description("Discover available project custom/dynamic properties for entity 'items', 'occurrences' or 'rooms'. Search matches ID, label or grouped title (case-insensitive substring); propertyGroup is an optional exact group filter. Omit search to list. Returns API IDs, labels, groups, types, units and known readOnly restrictions. Use pagination; never infer a field ID from a label. Call resolve_property first for user-facing labels: this custom-only keyword search must not override a verified built-in match or resolve built-in ambiguity.")]
    public Task<ReadResult> SearchCustomProperties(string entity, string? search = null, string? propertyGroup = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchCustomPropertiesAsync(entity, search, propertyGroup, limit, offset, cancellationToken);

    [McpServerTool(Name = "resolve_custom_property", ReadOnly = true, UseStructuredContent = true)]
    [Description("Resolve a project custom/dynamic property to its API ID for 'items', 'occurrences' or 'rooms'. Matches exact ID first, then exact label or 'group: label' title, case-insensitively. Optional propertyGroup narrows by exact group. Returns resolved, ambiguous or not_found with paginated candidates; ambiguous results never select an ID. Call resolve_property first: this custom-only fallback must not override built-in matches. Use search_custom_properties for partial custom names only after built-in lookup fails.")]
    public Task<ReadResult> ResolveCustomProperty(string entity, string property, string? propertyGroup = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.ResolveCustomPropertyAsync(entity, property, propertyGroup, limit, offset, cancellationToken);

    [McpServerTool(Name = "resolve_property", ReadOnly = true, UseStructuredContent = true)]
    [Description("Resolve a user-facing property for entity 'items', 'occurrences', 'rooms' or 'systems'. Always use this before custom or keyword search. Checks exact API names, then verified built-in model/display aliases, documented built-in synonyms, exact custom labels, and finally fuzzy suggestions. Aliases are generated from API schema, C# JsonPropertyName mappings, XML summaries and live metadata; sources are returned. Only a unique high-confidence match returns resolvedId. Ambiguous matches and fuzzy suggestions require user clarification, even if pagination shows one candidate. Room fields can be read with get_room and writable fields changed with update_room under write enablement/approval. Resolution does not enable System read/write tools.")]
    public Task<ReadResult> ResolveProperty(string entity, string property, int limit = 25, int offset = 0,
        CancellationToken cancellationToken = default) =>
        service.ResolvePropertyAsync(entity, property, limit, offset, cancellationToken);
}
