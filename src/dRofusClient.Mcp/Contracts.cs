using System.ComponentModel;
using System.Text.Json;

namespace dRofusClient.Mcp;

public sealed record FieldFilter(
    [property: Description("API field identifier, not a display label.")] string Field,
    [property: Description("eq, ne, lt, le, gt, ge, contains, startswith or endswith.")] string Operator,
    [property: Description("A scalar JSON value. Strings are escaped by the server.")] JsonElement Value);

public sealed record StatusChange(
    [property: Description("Positive occurrence status type ID.")] int StatusTypeId,
    [property: Description("Set either statusId or code, not both.")] int? StatusId = null,
    string? Code = null);

public sealed record FieldDefinition(string Id, string? Name, string Type, bool? ReadOnly, string? Unit, JsonElement? Schema);
public sealed record ReadResult(ProjectContext Project, object Data, int? NextOffset = null,
    bool HasMore = false, string? Notice = null);
public sealed record WriteResult(ProjectContext Project, string Outcome, int? Id,
    IReadOnlyList<string> CompletedSteps, object? Data = null, string? Notice = null);
public sealed record WriteProposal(ProjectContext Project, string Operation, int? Id,
    IReadOnlyDictionary<string, JsonElement> Changes, IReadOnlyList<StatusChange> Statuses, object? Before);
