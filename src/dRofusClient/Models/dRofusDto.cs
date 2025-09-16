using System.Reflection;

// ReSharper disable InconsistentNaming

namespace dRofusClient.Models;

public record dRofusIdDto : dRofusDto
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }
    /// <summary>Field name for Id, used in filters and order by clauses.</summary>
    /// <returns>"id"</returns> 
    public const string IdField = "id";

    public int GetId()
    {
        if (Id.HasValue)
            return Id.Value;
        throw new InvalidOperationException("Id is not set.");
    }
}


public record dRofusDto
{
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

    /// <summary>
    /// Sets the value of a specified property on the current object or adds it to the additional properties collection
    /// if the property does not exist.
    /// </summary>
    /// <remarks>If the specified property exists and is writable, its value is updated. If the property does
    /// not exist, the key-value pair is added to the  <c>AdditionalProperties</c> collection. The method does nothing
    /// if <paramref name="property"/> is <see langword="null"/> or empty.</remarks>
    /// <param name="property">The name of the property to set. This can be the actual property name or a JSON property name.</param>
    /// <param name="value">The value to assign to the specified property. The value will be converted to the property's type if necessary.</param>
    public void Set(string property, object value)
    {
        if (string.IsNullOrEmpty(property))
            return;
        var type = this.GetType();
        var propertyInfo = type.GetProperty(property) ?? GetPropertyByJsonPropertyName(type, property);
        if (propertyInfo is not null && propertyInfo.CanWrite)
        {
            var convertedValue = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(this, convertedValue);
        }
        else
        {
            AdditionalProperties[property] = value;
        }
    }
}