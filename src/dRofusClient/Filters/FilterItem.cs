using dRofusClient.AttributeConfigurations;
using System.Collections;

namespace dRofusClient.Filters;
public record FilterItem(string Field, Comparison Comparison, object? Value)
{
    public override string ToString()
    {
        return Comparison switch
        {
            Comparison.Eq => ReturnComparison("eq"),
            Comparison.Ne => ReturnComparison("ne"),
            Comparison.Lt => ReturnComparison("lt"),
            Comparison.Gt => ReturnComparison("gt"),
            Comparison.Le => ReturnComparison("le"),
            Comparison.Ge => ReturnComparison("ge"),
            Comparison.In => $"{Field} in ({ReturnInValues(Value)})",
            Comparison.Contains => $"contains({Field},{ConvertValue(Value)})",
            Comparison.StartsWith => $"startswith({Field},{ConvertValue(Value)})",
            Comparison.EndsWith => $"endswith({Field},{ConvertValue(Value)})",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string ReturnComparison(string comparison)
    {
        return $"{Field} {comparison} {ConvertValue(Value)}";
    }

    private string ReturnInValues(object? value)
    {
        // Prevent treating string as IEnumerable
        if (value is null || value is string)
            return ConvertValue(value);

        if (value is not IEnumerable enumerable)
            return ConvertValue(value);

        var results = new List<string>();

        foreach (var item in enumerable)
        {
            if (item is null)
                continue;

            // Prevent treating string as IEnumerable
            if (item is string)
            {
                results.Add(ConvertValue(item));
            }
            else if (item is IEnumerable innerEnumerable && item is not string)
            {
                results.AddRange(innerEnumerable.Cast<object>().Select(ConvertValue));
            }
            else
            {
                results.Add(ConvertValue(item));
            }
        }

        return string.Join(",", results);
    }

    private static string ConvertValue(object? value)
    {
        if (value is null)
            return "null";

        return value switch
        {
            DateTime dateTime => ConvertValue(dateTime.ToString("yyyy-M-d")),
            bool boolValue => boolValue.ToString().ToLower(),
            string stringValue => $"'{stringValue}'",
            AttributeConfigType attributeConfigType => attributeConfigType.ToRequest(),
            _ => value.ToString()
        };
    }
}