using System.Text.Json;

namespace dRofusClient.Extensions;

public static class JsonPropertyStatusExtensions
{
    public static dRofusStatusPatchOptions ToStatusPatchOption(this JsonProperty property)
    {
        var body = property.GetStatusPatchBody();

        if (body is null)
            throw new ArgumentException($"Invalid status body for property: {property.Name}", nameof(property));

        return new dRofusStatusPatchOptions
        {
            PropertyName = property.Name,
            Body = body
        };
    }

    public static dRofusStatusPatchBody? GetStatusPatchBody(this JsonProperty property)
    {
        if (property.Name.EndsWith("id_code", StringComparison.OrdinalIgnoreCase))
        {
            return new dRofusStatusPatchBody
            {
                Code = property.Value.GetString() ?? string.Empty
            };
        }

        if (property.Name.EndsWith("id_id", StringComparison.OrdinalIgnoreCase))
        {
            return new dRofusStatusPatchBody
            {
                StatusId = property.Value.GetInt32()
            };
        }

        if (property.Value.ValueKind == JsonValueKind.String)
        {
            return new dRofusStatusPatchBody
            {
                Code = property.Value.GetString() ?? string.Empty
            };
        }

        if (property.Value.ValueKind == JsonValueKind.Number)
        {
            return new dRofusStatusPatchBody
            {
                StatusId = property.Value.GetInt32()
            };
        }

        return null;
    }
}


