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
public sealed record CustomPropertyDefinition(string Id, string Name, string? PropertyGroup,
    string Title, string Type, string DataType, bool? ReadOnly, string? Unit);
public sealed record PropertyResolution(string Status, string? ResolvedId, int MatchCount,
    IReadOnlyList<CustomPropertyDefinition> Candidates);
public sealed record PropertyAlias(string Name, string Source, bool IsSynonym = false);
public sealed record VerifiedProperty(FieldDefinition Field, bool BuiltIn, IReadOnlyList<PropertyAlias> Aliases);
public sealed record VerifiedPropertyResolution(string Status, string Stage, string? ResolvedId, int MatchCount,
    IReadOnlyList<VerifiedProperty> Candidates);
public sealed record ReadResult(ProjectContext Project, object Data, int? NextOffset = null,
    bool HasMore = false, string? Notice = null);
public sealed record AttributeMappingResult(IReadOnlyDictionary<string, JsonElement> Configuration,
    string Status, int? MatchCount, IReadOnlyList<JsonElement> Mappings);
public sealed record WriteResult(ProjectContext Project, string Outcome, int? Id,
    IReadOnlyList<string> CompletedSteps, object? Data = null, string? Notice = null);
public sealed record WriteProposal(ProjectContext Project, string Operation, int? Id,
    IReadOnlyDictionary<string, JsonElement> Changes, IReadOnlyList<StatusChange> Statuses, object? Before);
