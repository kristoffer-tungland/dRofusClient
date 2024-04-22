using System.Reflection;

// ReSharper disable InconsistentNaming

namespace dRofusClient.Models;

public record dRofusDto
{
    // Dictionary to hold additional properties
    [JsonExtensionData] public Dictionary<string, object> AdditionalProperties { get; set; } = [];

    public string? GetPropertyAsString(string property) => GetProperty<string>(property);
    public int GetPropertyAsInt(string property) => GetProperty<int>(property);
    public bool GetPropertyAsBool(string property) => GetProperty<bool>(property);
    public double GetPropertyAsDouble(string property) => GetProperty<double>(property);

    public T? GetProperty<T>(string property)
    {
        var value = GetProperty(property);

        if (value is null)
            return default;

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public object? GetProperty(string property)
    {
        return AdditionalProperties[property] ?? GetPropertyByReflection(property);
    }

    object? GetPropertyByReflection(string property)
    {
        var type = this.GetType();
        var propertyInfo = type.GetProperty(property) ?? GetPropertyByJsonProperty(type, property);
        return propertyInfo?.GetValue(this);

    }

    static PropertyInfo? GetPropertyByJsonProperty(Type type, string property)
    {
        var properties = type.GetProperties();
        foreach (var propertyInfo in properties)
        {
            var jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty?.PropertyName == property)
                return propertyInfo;
        }

        return null;
    }
}