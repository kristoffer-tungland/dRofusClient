using System.ComponentModel;
using ModelContextProtocol.Server;

namespace dRofusClient.Mcp;

[McpServerToolType]
public sealed class ReadTools(DrofusService service)
{
    [McpServerTool(Name = "search_items", ReadOnly = true, UseStructuredContent = true)]
    [Description("Find Items (articles) in the configured project. Filters are ANDed. Returns a bounded page; use nextOffset while hasMore is true.")]
    public Task<ReadResult> SearchItems(FieldFilter[]? filters = null, string[]? fields = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchAsync("items", filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_item", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read an Item (article) by positive ID. Specify API field identifiers for extra fields; default is a compact summary.")]
    public Task<ReadResult> GetItem(int id, string[]? fields = null, CancellationToken cancellationToken = default) =>
        service.GetAsync("items", id, fields, cancellationToken);

    [McpServerTool(Name = "search_occurrences", ReadOnly = true, UseStructuredContent = true)]
    [Description("Find Occurrences, for example using article_id or room_id filters. Filters are ANDed. Returns a bounded page.")]
    public Task<ReadResult> SearchOccurrences(FieldFilter[]? filters = null, string[]? fields = null,
        int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.SearchAsync("occurrences", filters, fields, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_occurrence", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read an Occurrence by positive ID. An occurrence references an Item via article_id. Specify fields for additional data.")]
    public Task<ReadResult> GetOccurrence(int id, string[]? fields = null, CancellationToken cancellationToken = default) =>
        service.GetAsync("occurrences", id, fields, cancellationToken);

    [McpServerTool(Name = "get_item_history", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read the Item's available API change log, oldest first, including old/new values, user, time, action and notes. Not a complete historical snapshot.")]
    public Task<ReadResult> GetItemHistory(int id, DateTimeOffset? from = null, DateTimeOffset? to = null,
        FieldFilter[]? filters = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.HistoryAsync("items", id, from, to, filters, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_occurrence_history", ReadOnly = true, UseStructuredContent = true)]
    [Description("Read the Occurrence's available API change log, oldest first. Date bounds are inclusive; filters use log fields such as username, action or field.")]
    public Task<ReadResult> GetOccurrenceHistory(int id, DateTimeOffset? from = null, DateTimeOffset? to = null,
        FieldFilter[]? filters = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default) =>
        service.HistoryAsync("occurrences", id, from, to, filters, limit, offset, cancellationToken);

    [McpServerTool(Name = "get_field_metadata", ReadOnly = true, UseStructuredContent = true)]
    [Description("Discover field IDs, labels, types, units and known readOnly restrictions for entity 'items' or 'occurrences'. readOnly=null cannot be written.")]
    public Task<ReadResult> GetFieldMetadata(string entity, int limit = 100, int offset = 0,
        CancellationToken cancellationToken = default) =>
        service.MetadataAsync(entity, limit, offset, cancellationToken);
}
