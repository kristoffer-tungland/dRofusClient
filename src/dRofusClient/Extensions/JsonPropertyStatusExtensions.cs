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

    public static dRofusStatusPatchOptions ToStatusPatchOption(this KeyValuePair<string, object> property)
    {
        var body = property.GetStatusPatchBody();

        if (body is null)
            throw new ArgumentException($"Invalid status body for property: {property.Key}", nameof(property));

        return new dRofusStatusPatchOptions
        {
            PropertyName = property.Key,
            Body = body
        };
    }

    public static dRofusStatusPatchBody? GetStatusPatchBody(this KeyValuePair<string, object> property)
    {
        if (property.Key.EndsWith("id_code", StringComparison.OrdinalIgnoreCase))
        {
            return new dRofusStatusPatchBody
            {
                Code = property.Value?.ToString() ?? string.Empty
            };
        }

        if (property.Key.EndsWith("id_id", StringComparison.OrdinalIgnoreCase))
        {
            if (property.Value is int intVal)
            {
                return new dRofusStatusPatchBody
                {
                    StatusId = intVal
                };
            }
            if (int.TryParse(property.Value?.ToString(), out var parsed))
            {
                return new dRofusStatusPatchBody
                {
                    StatusId = parsed
                };
            }
        }

        if (property.Value is string strVal)
        {
            return new dRofusStatusPatchBody
            {
                Code = strVal
            };
        }

        if (property.Value is int intVal2)
        {
            return new dRofusStatusPatchBody
            {
                StatusId = intVal2
            };
        }

        return null;
    }
}


