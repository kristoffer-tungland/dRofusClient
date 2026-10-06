using System.ComponentModel;
using ModelContextProtocol.Server;

namespace dRofusClient.Mcp;

[McpServerToolType]
public sealed class ReadTools(DrofusService service)
{
    [McpServerTool(Name = "search_items", ReadOnly = true, UseStructuredContent = true)]
    [Description("Find Items (articles) in the configured project. Filters are ANDed. Returns a bounded page; use nextOffset while hasMore is true. " +
        "For user-requested properties, proactively use get_field_metadata (entity='items') for standard/custom fields, search_custom_properties for partial labels or groups, and resolve_custom_property for exact custom names. Use returned IDs verbatim in fields and filter.field; include requested properties in fields. Never guess IDs; clarify ambiguous matches.")]
    public Task<ReadResult> SearchItems(FieldFilter[]? filters = null, string[]? fields = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchAsync("items", filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_item", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read an Item (article) by positive ID. Specify API field identifiers for extra fields; default is a compact summary. " +
        "For user-requested properties, proactively use get_field_metadata (entity='items') for standard/custom fields, search_custom_properties for partial labels or groups, and resolve_custom_property for exact custom names. Include the returned IDs verbatim in fields; otherwise requested properties may be absent. Never guess IDs; clarify ambiguous matches.")]
    public Task<ReadResult> GetItem(int id, string[]? fields = null, CancellationToken cancellationToken = default) =>
        service.GetAsync("items", id, fields, cancellationToken);

    [McpServerTool(Name = "search_occurrences", ReadOnly = true, UseStructuredContent = true)]
    [Description("Find Occurrences, for example using article_id or room_id filters. Filters are ANDed. Returns a bounded page. " +
        "For user-requested properties, proactively use get_field_metadata (entity='occurrences') for standard/custom fields, search_custom_properties for partial labels or groups, and resolve_custom_property for exact custom names. Use returned IDs verbatim in fields and filter.field; include requested properties in fields. Never guess IDs; clarify ambiguous matches.")]
    public Task<ReadResult> SearchOccurrences(FieldFilter[]? filters = null, string[]? fields = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchAsync("occurrences", filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_occurrence", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read an Occurrence by positive ID. An occurrence references an Item via article_id. Specify fields for additional data. " +
        "For user-requested properties, proactively use get_field_metadata (entity='occurrences') for standard/custom fields, search_custom_properties for partial labels or groups, and resolve_custom_property for exact custom names. Include the returned IDs verbatim in fields; otherwise requested properties may be absent. Never guess IDs; clarify ambiguous matches.")]
    public Task<ReadResult> GetOccurrence(int id, string[]? fields = null, CancellationToken cancellationToken = default) =>
        service.GetAsync("occurrences", id, fields, cancellationToken);

    [McpServerTool(Name = "get_item_history", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read the Item's available API change log, oldest first, including old/new values, user, time, action and notes. Not a complete historical snapshot. " +
        "To identify a user-named property, proactively consult get_field_metadata, search_custom_properties or resolve_custom_property with entity='items'. Clarify ambiguous matches. History filters target log fields, not entity property IDs; inspect returned log entries to determine how a property is represented in the log's field value.")]
    public Task<ReadResult> GetItemHistory(int id, DateTimeOffset? from = null, DateTimeOffset? to = null,
        FieldFilter[]? filters = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.HistoryAsync("items", id, from, to, filters, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_occurrence_history", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read the Occurrence's available API change log, oldest first. Date bounds are inclusive; filters use log fields such as username, action or field. " +
        "To identify a user-named property, proactively consult get_field_metadata, search_custom_properties or resolve_custom_property with entity='occurrences'. Clarify ambiguous matches. History filters target log fields, not entity property IDs; inspect returned log entries to determine how a property is represented in the log's field value.")]
    public Task<ReadResult> GetOccurrenceHistory(int id, DateTimeOffset? from = null, DateTimeOffset? to = null,
        FieldFilter[]? filters = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.HistoryAsync("occurrences", id, from, to, filters, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_field_metadata", ReadOnly = true, UseStructuredContent = true)]
    [Description("Discover field IDs, labels, types, units and known readOnly restrictions for entity 'items' or 'occurrences'. readOnly=null cannot be written.")]
    public Task<ReadResult> GetFieldMetadata(string entity, int limit = 100, int offset = 0,
        CancellationToken cancellationToken = default) =>
        service.MetadataAsync(entity, limit, offset, cancellationToken);

    [McpServerTool(Name = "search_custom_properties", ReadOnly = true, UseStructuredContent = true)]
    [Description("Discover available project custom/dynamic properties for entity 'items' or 'occurrences'. Search matches ID, label or grouped title (case-insensitive substring); propertyGroup is an optional exact group filter. Omit search to list. Returns API IDs, labels, groups, types, units and known readOnly restrictions. Use pagination; never infer a field ID from a label.")]
    public Task<ReadResult> SearchCustomProperties(string entity, string? search = null, string? propertyGroup = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchCustomPropertiesAsync(entity, search, propertyGroup, limit, offset, cancellationToken);

    [McpServerTool(Name = "resolve_custom_property", ReadOnly = true, UseStructuredContent = true)]
    [Description("Resolve a project custom/dynamic property to its API ID for 'items' or 'occurrences'. Matches exact ID first, then exact label or 'group: label' title, case-insensitively. Optional propertyGroup narrows by exact group. Returns resolved, ambiguous or not_found with paginated candidates; ambiguous results never select an ID. Use search_custom_properties for partial names.")]
    public Task<ReadResult> ResolveCustomProperty(string entity, string property, string? propertyGroup = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.ResolveCustomPropertyAsync(entity, property, propertyGroup, limit, offset, cancellationToken);
}
