using System.Text.Json;

namespace dRofusClient.Extensions;

public static class JsonPropertyStatusExtensions
{
    public static StatusPatchRequest ToStatusPatchOption(this JsonProperty property)
    {
        var body = property.GetStatusPatchBody();

        if (body is null)
            throw new ArgumentException($"Invalid status body for property: {property.Name}", nameof(property));

        return new StatusPatchRequest
        {
            PropertyName = property.Name,
            Body = body
        };
    }

    public static StatusPatchBody? GetStatusPatchBody(this JsonProperty property)
    {
        if (property.Name.EndsWith("id_code", StringComparison.OrdinalIgnoreCase))
        {
            return new StatusPatchBody
            {
                Code = property.Value.GetString() ?? string.Empty
            };
        }

        if (property.Name.EndsWith("id_id", StringComparison.OrdinalIgnoreCase))
        {
            return new StatusPatchBody
            {
                StatusId = property.Value.GetInt32()
            };
        }

        if (property.Value.ValueKind == JsonValueKind.String)
        {
            return new StatusPatchBody
            {
                Code = property.Value.GetString() ?? string.Empty
            };
        }

        if (property.Value.ValueKind == JsonValueKind.Number)
        {
            return new StatusPatchBody
            {
                StatusId = property.Value.GetInt32()
            };
        }

        return null;
    }

    public static StatusPatchRequest ToStatusPatchOption(this KeyValuePair<string, object> property)
    {
        var body = property.GetStatusPatchBody();

        if (body is null)
            throw new ArgumentException($"Invalid status body for property: {property.Key}", nameof(property));

        return new StatusPatchRequest
        {
            PropertyName = property.Key,
            Body = body
        };
    }

    public static StatusPatchBody? GetStatusPatchBody(this KeyValuePair<string, object> property)
    {
        if (property.Key.EndsWith("id_code", StringComparison.OrdinalIgnoreCase))
        {
            return new StatusPatchBody
            {
                Code = property.Value?.ToString() ?? string.Empty
            };
        }

        if (property.Key.EndsWith("id_id", StringComparison.OrdinalIgnoreCase))
        {
            if (property.Value is int intVal)
            {
                return new StatusPatchBody
                {
                    StatusId = intVal
                };
            }
            if (int.TryParse(property.Value?.ToString(), out var parsed))
            {
                return new StatusPatchBody
                {
                    StatusId = parsed
                };
            }
        }

        if (property.Value is string strVal)
        {
            return new StatusPatchBody
            {
                Code = strVal
            };
        }

        if (property.Value is int intVal2)
        {
            return new StatusPatchBody
            {
                StatusId = intVal2
            };
        }

        return null;
    }
}


