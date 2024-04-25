using System.Reflection;

namespace dRofusClient;

public static class dRofusFieldsOptionsExtensions
{
    public static TOption Select<TOption>(this TOption options, string field)
        where TOption : dRofusFieldsOptions
    {
        return options.Select([field]);
    }

    public static TOption Select<TOption>(this TOption options, IEnumerable<string> fields)
        where TOption : dRofusFieldsOptions
    {
        options._fieldsToSelect.AddRange(fields.Select(x => x.ToSnakeCase()));
        return options;
    }

    public static TOption Select<TOption>(this TOption options, Type typeToSelectPropertiesFrom)
        where TOption : dRofusFieldsOptions
    {
        // Get all public properties from the type
        var properties = typeToSelectPropertiesFrom.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var propertyInfo in properties)
        {
            // Get JsonNameAttribute from the property
           if (propertyInfo.GetCustomAttribute<JsonExtensionDataAttribute>() is not null)
                continue;

            options._fieldsToSelect.Add(propertyInfo.Name.ToSnakeCase());
        }

        return options;
    }
}