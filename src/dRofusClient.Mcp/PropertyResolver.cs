using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using dRofusClient.PropertyMeta;

namespace dRofusClient.Mcp;

public sealed class PropertyResolver(FieldCatalog catalog)
{
    private readonly Dictionary<string, string> _summaries = LoadSummaries();

    public VerifiedPropertyResolution Resolve(string schema, IReadOnlyList<dRofusPropertyMeta> metadata,
        string query, int limit, int offset)
    {
        var properties = BuildProperties(schema, metadata);
        var name = Normalize(query);
        var exact = properties.Where(p => string.Equals(p.Field.Id, query.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        if (exact.Count > 0)
            return Result(exact, "api_name", limit, offset);

        var builtIn = properties.Where(p => p.BuiltIn).ToList();
        var aliases = builtIn.Where(p => p.Aliases.Any(a => !a.IsSynonym && Normalize(a.Name) == name)).ToList();
        var synonyms = builtIn.Where(p => p.Aliases.Any(a => a.IsSynonym && Normalize(a.Name) == name)).ToList();
        // A conflicting documented synonym is still a high-confidence match, not a tie to silently discard.
        var verified = aliases.Concat(synonyms).DistinctBy(p => p.Field.Id).ToList();
        if (verified.Count > 0)
            return Result(verified, aliases.Count > 0 ? "built_in_alias" : "built_in_synonym", limit, offset);

        var custom = properties.Where(p => !p.BuiltIn && p.Aliases.Any(a => Normalize(a.Name) == name)).ToList();
        if (custom.Count > 0)
            return Result(custom, "custom", limit, offset);

        // Fuzzy results are suggestions only, even when there is just one candidate.
        var suggestions = properties.Select(p => new
        {
            Property = p,
            Distance = p.Aliases.Select(a => Normalize(a.Name)).Append(Normalize(p.Field.Id))
                .Where(alias => alias.Length <= 256)
                .Select(alias => Distance(name, alias)).DefaultIfEmpty(int.MaxValue).Min()
        }).Where(p => name.Length >= 4 && p.Distance <= (name.Length >= 8 ? 2 : 1))
            .OrderBy(p => p.Distance).ThenByDescending(p => p.Property.BuiltIn)
            .ThenBy(p => p.Property.Field.Id, StringComparer.Ordinal).Select(p => p.Property).ToList();
        return suggestions.Count > 0
            ? new("suggestions", "fuzzy", null, suggestions.Count, suggestions.Skip(offset).Take(limit).ToArray())
            : new("not_found", "none", null, 0, []);
    }

    private List<VerifiedProperty> BuildProperties(string schema, IReadOnlyList<dRofusPropertyMeta> metadata)
    {
        var fields = catalog.WithMetadata(schema, metadata);
        var standard = catalog.GetFields(schema);
        var metadataById = metadata.ToLookup(m => m.Id, StringComparer.Ordinal);
        var model = FieldCatalog.GetModelType(schema)!;
        var modelProperties = model.GetProperties()
            .Where(p => p.GetCustomAttribute<JsonPropertyNameAttribute>() is not null)
            .ToDictionary(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name, StringComparer.Ordinal);
        foreach (var (id, property) in modelProperties)
        {
            if (!fields.ContainsKey(id))
                fields[id] = new(id, null, ModelType(property.PropertyType), null, null, null);
            else if (fields[id].Type == "unknown")
                fields[id] = fields[id] with { Type = ModelType(property.PropertyType) };
        }

        return fields.Values.OrderBy(f => f.Id, StringComparer.Ordinal).Select(field =>
        {
            var aliases = new List<PropertyAlias>();
            var builtIn = standard.ContainsKey(field.Id) || modelProperties.ContainsKey(field.Id);
            if (standard.TryGetValue(field.Id, out var definition) && definition.Schema is { } api)
            {
                AddSchemaAlias(api, "title", "OpenAPI title", false, aliases);
                AddSchemaAlias(api, "description", "OpenAPI description", true, aliases);
            }
            if (modelProperties.TryGetValue(field.Id, out var property))
            {
                var source = $"model:{property.DeclaringType!.FullName}.{property.Name}";
                aliases.Add(new(property.Name, source));
                aliases.Add(new(Humanize(property.Name), source));
                if (_summaries.TryGetValue($"P:{property.DeclaringType.FullName}.{property.Name}", out var summary))
                    AddLabel(summary, $"XML:{property.DeclaringType.FullName}.{property.Name}", true, aliases);
            }
            foreach (var meta in metadataById[field.Id])
            {
                AddLabel(meta.Name, "API metadata name", false, aliases);
                if (!string.IsNullOrWhiteSpace(meta.PropertyGroup))
                    aliases.Add(new(meta.GetTitle(), "API metadata group/name"));
            }
            return new VerifiedProperty(field, builtIn, aliases.Distinct().ToArray());
        }).ToList();
    }

    private static void AddSchemaAlias(JsonElement schema, string key, string source, bool synonym, List<PropertyAlias> aliases)
    {
        if (schema.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String)
            AddLabel(value.GetString(), source, synonym, aliases);
    }

    private static void AddLabel(string? value, string source, bool synonym, List<PropertyAlias> aliases)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        value = Regex.Replace(value.Trim(), @"\s+", " ");
        aliases.Add(new(value, source, synonym));
        // dRofus titles and XML labels use "Group: Label"; only remove that explicit group prefix.
        var colon = value.IndexOf(':');
        if (colon >= 0 && colon == value.LastIndexOf(':') && colon + 1 < value.Length)
            aliases.Add(new(value[(colon + 1)..].Trim(), source, synonym));
    }

    private static VerifiedPropertyResolution Result(List<VerifiedProperty> matches, string stage, int limit, int offset) =>
        new(matches.Count == 1 ? "resolved" : "ambiguous", stage,
            matches.Count == 1 ? matches[0].Field.Id : null, matches.Count,
            matches.OrderBy(p => p.Field.Id, StringComparer.Ordinal).Skip(offset).Take(limit).ToArray());

    private static string Normalize(string text) => Regex.Replace(text.Trim(), @"\s+", " ").ToUpperInvariant();

    private static string Humanize(string name) =>
        Regex.Replace(Regex.Replace(name, "([A-Z]+)([A-Z][a-z])", "$1 $2"), "([a-z0-9])([A-Z])", "$1 $2");

    private static string ModelType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "string";
        if (type == typeof(int) || type == typeof(long)) return "integer";
        if (type == typeof(double) || type == typeof(decimal) || type == typeof(float)) return "number";
        if (type == typeof(bool)) return "boolean";
        return "unknown";
    }

    private static Dictionary<string, string> LoadSummaries()
    {
        var path = Path.ChangeExtension(typeof(dRofusPropertyMeta).Assembly.Location, ".xml");
        if (!File.Exists(path))
            return new(StringComparer.Ordinal);
        return XDocument.Load(path).Descendants("member")
            .Where(m => m.Attribute("name") is not null && m.Element("summary") is not null)
            .ToDictionary(m => m.Attribute("name")!.Value, m => m.Element("summary")!.Value, StringComparer.Ordinal);
    }

    private static int Distance(string left, string right)
    {
        if (Math.Abs(left.Length - right.Length) > 2)
            return int.MaxValue;
        var previous = Enumerable.Range(0, right.Length + 1).ToArray();
        var current = new int[right.Length + 1];
        for (var i = 1; i <= left.Length; i++)
        {
            current[0] = i;
            for (var j = 1; j <= right.Length; j++)
                current[j] = Math.Min(Math.Min(current[j - 1] + 1, previous[j] + 1),
                    previous[j - 1] + (left[i - 1] == right[j - 1] ? 0 : 1));
            (previous, current) = (current, previous);
        }
        return previous[right.Length];
    }
}
