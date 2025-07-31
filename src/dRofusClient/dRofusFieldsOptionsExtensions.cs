using System.Reflection;

namespace dRofusClient;

public static class dRofusFieldsOptionsExtensions
{
    public static TOption Select<TOption>(this TOption options, string field)
        where TOption : ItemQuery
    {
        return options.Select([field]);
    }

    public static TOption Select<TOption>(this TOption options, IEnumerable<string> fields)
        where TOption : ItemQuery
    {
        if (fields is null)
            throw new ArgumentNullException(nameof(fields), "Fields cannot be null.");

        options._fieldsToSelect.AddRange(fields.Select(x => x.ToSnakeCase()));
        return options;
    }

    public static TOption Select<TOption>(this TOption options, params string[] fields)
        where TOption : ItemQuery
    {
        if (fields is null || fields.Length == 0)
            throw new ArgumentException("Fields cannot be null or empty.", nameof(fields));

        options._fieldsToSelect.AddRange(fields.Select(x => x.ToSnakeCase()));
        return options;
    }

    public static TOption Select<TOption>(this TOption options, Type typeToSelectPropertiesFrom)
        where TOption : ItemQuery
    {
        // Get all public properties from the type
        var properties = typeToSelectPropertiesFrom.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var propertyInfo in properties)
        {
            // Get JsonNameAttribute from the property
            if (propertyInfo.GetCustomAttribute<JsonExtensionDataAttribute>() is not null)
                continue;

            if (propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>() is JsonPropertyNameAttribute jsonPropertyNameAttribute)
            {
                options._fieldsToSelect.Add(jsonPropertyNameAttribute.Name);
                continue;
            }
            else
                options._fieldsToSelect.Add(propertyInfo.Name.ToSnakeCase());
        }

        return options;
    }
}