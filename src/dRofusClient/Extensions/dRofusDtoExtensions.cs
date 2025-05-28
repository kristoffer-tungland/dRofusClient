using System.Text.Json;

namespace dRofusClient.Extensions;

public static class dRofusDtoExtensions
{
    public static dRofusPatchOptions ToPatchOption(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdField(json, out var statusFields);
        
        return new dRofusPatchOptions
        {
            Body = json,
            StatusFields = statusFields
        };
    }

    public static dRofusPostOptions ToPostOption(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdField(json, out var statusFields);

        return new dRofusPostOptions
        {
            Body = json,
            StatusFields = statusFields
        };
    }

    private static string RemoveIdField(string json, out List<JsonProperty>? statusFields)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        statusFields = null;

        // Copy all properties except "id" to a new dictionary
        var dict = new Dictionary<string, JsonElement>();
        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Name.StartsWith("ce", StringComparison.OrdinalIgnoreCase))
            {
                if (statusFields is null)
                    statusFields = [];

                statusFields.Add(prop);
                continue;
            }

            if (prop.Name.StartsWith("occurrence_classification_", StringComparison.OrdinalIgnoreCase))
            {
                if (statusFields is null)
                    statusFields = [];

                statusFields.Add(prop);
                continue;
            }

            if (!string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase))
                dict[prop.Name] = prop.Value;
        }

        if (statusFields is null)
            doc.Dispose();

        // Serialize the dictionary back to JSON
        return JsonSerializer.Serialize(dict, Json.Options);
    }
}