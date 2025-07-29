using System.Reflection;
using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace dRofusClient.Models;

public record dRofusDto
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }
    /// <summary>Field name for Id, used in filters and order by clauses.</summary>
    /// <returns>"id"</returns> 
    public static string IdField => "id";

    // Dictionary to hold additional properties
    [JsonExtensionData]
    public Dictionary<string, object> AdditionalProperties { get; set; } = [];

    public string? GetPropertyAsString(string property) => GetProperty<string>(property);
    public int GetPropertyAsInt(string property) => GetProperty<int>(property);
    public bool GetPropertyAsBool(string property) => GetProperty<bool>(property);
    public double GetPropertyAsDouble(string property) => GetProperty<double>(property);

    public T? GetProperty<T>(string property)
    {
        var value = GetProperty(property);

        if (value is null)
            return default;

        // If the target type is string, just cast or call ToString
        if (typeof(T) == typeof(string))
        {
            // If value is already a string, return it directly
            if (value is string s)
                return (T)(object)s;

            // Otherwise, use ToString
            return (T)(object)value.ToString();
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public object? GetProperty(string property)
    {
        if (string.IsNullOrEmpty(property))
            return null;

        if (AdditionalProperties.ContainsKey(property))
            return AdditionalProperties[property];

        return GetPropertyByReflection(property);
    }

    private object? GetPropertyByReflection(string property)
    {
        var type = this.GetType();
        var propertyInfo = type.GetProperty(property) ?? GetPropertyByJsonPropertyName(type, property);
        return propertyInfo?.GetValue(this);
    }

    private static PropertyInfo? GetPropertyByJsonPropertyName(Type type, string property)
    {
        var properties = type.GetProperties();
        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.Name.ToSnakeCase() == property)
                return propertyInfo;
        }

        return null;
    }
}