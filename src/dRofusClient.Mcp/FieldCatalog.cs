using System.Text.Json;
using System.Text.RegularExpressions;
using dRofusClient.PropertyMeta;
using ModelContextProtocol;

namespace dRofusClient.Mcp;

public sealed class FieldCatalog
{
    private readonly JsonElement _schemas;

    public FieldCatalog()
    {
        using var stream = typeof(FieldCatalog).Assembly.GetManifestResourceStream("dRofus.OpenApi.json")!;
        using var document = JsonDocument.Parse(stream);
        _schemas = document.RootElement.GetProperty("components").GetProperty("schemas").Clone();
    }

    public Dictionary<string, FieldDefinition> GetFields(string schemaName)
    {
        var result = new Dictionary<string, FieldDefinition>(StringComparer.Ordinal);
        AddSchema(_schemas.GetProperty(schemaName), result);
        return result;
    }

    public Dictionary<string, FieldDefinition> WithMetadata(string schemaName, IEnumerable<dRofusPropertyMeta> metadata)
    {
        var fields = GetFields(schemaName);
        foreach (var field in metadata)
        {
            if (!IsFieldName(field.Id))
                continue;
            if (fields.TryGetValue(field.Id, out var known))
                fields[field.Id] = known with { Name = field.Name, Unit = field.Unit };
            else
            {
                bool? readOnly = field.AdditionalProperties.TryGetValue("readOnly", out var value) &&
                    value is JsonElement element && element.ValueKind is JsonValueKind.True or JsonValueKind.False
                        ? element.GetBoolean() : null;
                fields[field.Id] = new(field.Id, field.Name, NormalizeType(field.DataType), readOnly, field.Unit, null);
            }
        }
        return fields;
    }

    public static void ValidateChanges(IReadOnlyDictionary<string, JsonElement> changes,
        IReadOnlyDictionary<string, FieldDefinition> fields, bool creating = false)
    {
        if (changes.Count > 50 || JsonSerializer.SerializeToUtf8Bytes(changes).Length > 16_384)
            throw new McpException("A write is limited to 50 fields and 16 KiB.");
        foreach (var (name, value) in changes)
        {
            if (!fields.TryGetValue(name, out var field) || field.ReadOnly != false)
                throw new McpException("A field is unknown, read-only, or has unverified write permissions. Inspect get_field_metadata.");
            if (value.ValueKind == JsonValueKind.Null)
            {
                if (creating && !(field.Schema?.TryGetProperty("nullable", out var nullable) == true && nullable.GetBoolean()))
                    throw new McpException("A required creation field cannot be null.");
                continue;
            }
            var valid = field.Type switch
            {
                "string" => value.ValueKind == JsonValueKind.String,
                "integer" => value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out _),
                "number" => value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out _),
                "boolean" => value.ValueKind is JsonValueKind.True or JsonValueKind.False,
                "array" => value.ValueKind == JsonValueKind.Array && value.GetArrayLength() <= 100 &&
                    value.EnumerateArray().All(x => x.ValueKind == JsonValueKind.String),
                _ => false
            };
            if (!valid)
                throw new McpException("A field value has the wrong JSON type.");
            if (field.Schema?.TryGetProperty("maxLength", out var maxLength) == true &&
                value.ValueKind == JsonValueKind.String && value.GetString()!.Length > maxLength.GetInt32())
                throw new McpException("A field exceeds its maximum length.");
            if (field.Type == "integer" && name.EndsWith("_id", StringComparison.Ordinal) && value.TryGetInt32(out var id) && id <= 0)
                throw new McpException("Reference IDs must be positive.");
        }
    }

    public static bool IsFieldName(string? name) => name is not null &&
        Regex.IsMatch(name, "^[a-z][a-z0-9_]{0,127}$", RegexOptions.CultureInvariant);

    private void AddSchema(JsonElement schema, Dictionary<string, FieldDefinition> fields)
    {
        if (schema.TryGetProperty("allOf", out var allOf))
            foreach (var part in allOf.EnumerateArray())
                if (part.TryGetProperty("$ref", out var reference))
                    AddSchema(_schemas.GetProperty(reference.GetString()!.Split('/')[^1]), fields);
        if (schema.TryGetProperty("properties", out var properties))
            foreach (var property in properties.EnumerateObject())
            {
                var value = property.Value;
                fields[property.Name] = new(property.Name,
                    value.TryGetProperty("description", out var description) ? description.GetString() : null,
                    value.GetProperty("type").GetString()!,
                    value.TryGetProperty("readOnly", out var readOnly) && readOnly.GetBoolean(), null, value.Clone());
            }
        if (schema.TryGetProperty("additionalProperties", out var additional) && additional.ValueKind == JsonValueKind.Object)
            AddSchema(additional, fields);
    }

    private static string NormalizeType(string type) => type.ToLowerInvariant() switch
    {
        "int" or "int32" or "integer" => "integer",
        "double" or "decimal" or "float" or "number" => "number",
        "bool" or "boolean" => "boolean",
        "string" or "text" => "string",
        _ => "unknown"
    };
}
