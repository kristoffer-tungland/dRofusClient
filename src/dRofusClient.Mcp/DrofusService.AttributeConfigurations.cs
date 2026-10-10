using System.Text.Json;
using dRofusClient.Enums;
using dRofusClient.Models;
using ModelContextProtocol;

namespace dRofusClient.Mcp;

public sealed partial class DrofusService
{
    private static readonly string[] ConfigurationFields =
        ["id", "name", "config_type", "applicable_to", "available_to_users", "is_default"];

    private const string MappingNotice =
        "Select the configuration for the intended application/entity; a default is not necessarily the active Revit configuration. " +
        "Clarify ambiguous configurations or mappings. Key identifies corresponding objects, not a transfer direction; " +
        "ToExternalApplication means dRofus to the external application, ToDrofus means the reverse. " +
        "Missing, null or unfamiliar directions are unknown: do not infer Key or a transfer direction. " +
        "Verify identifiers against dRofus endpoint metadata and the other MCP server's Revit parameter metadata, including type/instance scope and units. " +
        "Mappings express intended synchronization, not proof values are synchronized, and never bypass either server's write safeguards.";

    public Task<ReadResult> ListAttributeConfigurationsAsync(FieldFilter[]? filters, int limit, int offset,
        CancellationToken cancellationToken) => SafeAsync(async () =>
    {
        var query = BuildQuery(limit, offset, filters, ConfigurationFields).OrderBy("id");
        var configurations = await client.SendListAsync<dRofusDto>(
            HttpMethod.Get, dRofusType.AttributeConfigurations, query, cancellationToken);
        return Page(configurations.Select(c => ConfigurationSummary(JsonSerializer.SerializeToElement(c))).ToList(),
            limit, offset, MappingNotice);
    });

    public Task<ReadResult> GetAttributeConfigurationAsync(int id, int limit, int offset,
        CancellationToken cancellationToken) =>
        ReadAttributeMappingsAsync(id, null, "either", limit, offset, cancellationToken);

    public Task<ReadResult> FindAttributeMappingsAsync(int configurationId, string property, string side,
        int limit, int offset, CancellationToken cancellationToken) =>
        ReadAttributeMappingsAsync(configurationId, property, side, limit, offset, cancellationToken, search: true);

    private Task<ReadResult> ReadAttributeMappingsAsync(int id, string? property, string side, int limit, int offset,
        CancellationToken cancellationToken, bool search = false) => SafeAsync(async () =>
    {
        ValidateId(id);
        ValidatePage(limit, offset);
        if (side is not ("drofus" or "external" or "either"))
            throw new McpException("Side must be drofus, external or either.");
        if (search && (string.IsNullOrWhiteSpace(property) || property.Length > 1024))
            throw new McpException("A mapping property must be non-empty and at most 1024 characters.");
        var query = BuildQuery(1, 0, [new("id", "eq", JsonSerializer.SerializeToElement(id))],
            [.. ConfigurationFields, "elements"]);
        // Keep nullable/unknown directions as supplied by the API rather than defaulting the core enum to Key.
        var configurations = await client.SendListAsync<dRofusDto>(
            HttpMethod.Get, dRofusType.AttributeConfigurations, query, cancellationToken);
        if (configurations.Count == 0)
            throw new McpException("Attribute configuration was not found in the configured project.");
        if (configurations.Count != 1)
            throw new McpException("Multiple attribute configurations were returned for the ID. Clarify the configuration before proceeding.");
        var configuration = JsonSerializer.SerializeToElement(configurations[0]);
        var summary = ConfigurationSummary(configuration);
        if (!summary.TryGetValue("id", out var returnedId) || returnedId.ValueKind != JsonValueKind.Number ||
            !returnedId.TryGetInt32(out var actualId) || actualId != id)
            throw new McpException("The API returned an unexpected attribute configuration ID.");
        if (!configuration.TryGetProperty("elements", out var elements) || elements.ValueKind == JsonValueKind.Null)
            return new ReadResult(settings.Context, new AttributeMappingResult(summary, "unavailable", null, []),
                Notice: "Mapping elements were not supplied; this does not mean the configuration has no mappings. " + MappingNotice);
        if (elements.ValueKind != JsonValueKind.Array || elements.EnumerateArray().Any(e => e.ValueKind != JsonValueKind.Object))
            throw new McpException("The API returned an unexpected attribute mapping structure.");

        var matches = elements.EnumerateArray().ToArray();
        var status = "available";
        if (search)
        {
            string[] prefixes = side switch
            {
                "drofus" => ["drofus"],
                "external" => ["external"],
                _ => ["drofus", "external"]
            };
            var exactIds = matches.Where(e => prefixes.Any(p =>
                MatchesMapping(e, $"{p}_attribute_id", property!, StringComparison.Ordinal))).ToArray();
            matches = exactIds.Length > 0 ? exactIds : matches.Where(e => prefixes.Any(p =>
                MatchesMapping(e, $"{p}_attribute_label", property!, StringComparison.OrdinalIgnoreCase))).ToArray();
            status = matches.Length switch { 0 => "not_found", 1 => "matched", _ => "ambiguous" };
        }
        return new ReadResult(settings.Context,
            new AttributeMappingResult(summary, status, matches.Length, matches.Skip(offset).Take(limit).ToArray()),
            matches.Length > offset + limit ? offset + limit : null, matches.Length > offset + limit, MappingNotice);
    });

    private static bool MatchesMapping(JsonElement mapping, string field, string property, StringComparison comparison) =>
        mapping.TryGetProperty(field, out var value) && value.ValueKind == JsonValueKind.String &&
        string.Equals(value.GetString(), property, comparison);

    private static Dictionary<string, JsonElement> ConfigurationSummary(JsonElement configuration)
    {
        if (configuration.ValueKind != JsonValueKind.Object)
            throw new McpException("The API returned an unexpected attribute configuration structure.");
        return configuration.EnumerateObject().Where(p => ConfigurationFields.Contains(p.Name))
            .ToDictionary(p => p.Name, p => p.Value.Clone());
    }
}
