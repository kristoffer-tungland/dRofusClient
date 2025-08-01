using System.Text.Json;

namespace dRofusClient.Extensions;

public static class dRofusDtoExtensions
{
    public static PatchRequest ToPatchRequest(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdAndStatusFields(json, out var statusFields);

        return new PatchRequest
        {
            Body = json,
            StatusFields = statusFields
        };
    }

    public static PostRequest ToPostRequest(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdAndStatusFields(json, out var statusFields);

        return new PostRequest
        {
            Body = json,
            StatusFields = statusFields
        };
    }

    private static string RemoveIdAndStatusFields(string json, out Dictionary<string, object>? statusFields)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        statusFields = null;

        // Copy all properties except "id" to a new dictionary
        var dict = new Dictionary<string, JsonElement>();

        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Name.StartsWith("ce", StringComparison.OrdinalIgnoreCase) ||
                prop.Name.StartsWith("occurrence_classification_", StringComparison.OrdinalIgnoreCase))
            {
                if (statusFields is null)
                    statusFields = new Dictionary<string, object>();

                // Convert JsonElement to .NET object
                statusFields[prop.Name] = prop.Value.ValueKind switch
                {
                    JsonValueKind.String => prop.Value.GetString()!,
                    JsonValueKind.Number => prop.Value.TryGetInt32(out var i) ? i : prop.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => prop.Value.ToString()!
                };
                continue;
            }

            if (!string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase))
                dict[prop.Name] = prop.Value;
        }

        // Serialize the dictionary back to JSON
        return JsonSerializer.Serialize(dict, Json.Options);
    }
}