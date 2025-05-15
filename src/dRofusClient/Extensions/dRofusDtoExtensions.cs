using System.Text.Json;

namespace dRofusClient.Extensions;

public static class dRofusDtoExtensions
{
    public static dRofusBodyPatchOptions ToPatchOption(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdField(json);
        return new dRofusBodyPatchOptions(json);
    }

    public static dRofusBodyPostOptions ToPostOption(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdField(json);
        return new dRofusBodyPostOptions(json);
    }

    static string RemoveIdField(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Copy all properties except "id" to a new dictionary
        var dict = new Dictionary<string, JsonElement>();
        foreach (var prop in root.EnumerateObject())
        {
            if (!string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase))
                dict[prop.Name] = prop.Value;
        }

        // Serialize the dictionary back to JSON
        return JsonSerializer.Serialize(dict, Json.Options);
    }
}