using System.Collections;

namespace dRofusClient.Filters;

public record dRofusFilterItem(string Field, dRofusComparison Comparison, object? Value)
{
    public override string ToString()
    {
        return Comparison switch
        {
            dRofusComparison.Eq => ReturnComparison("eq"),
            dRofusComparison.Ne => ReturnComparison("ne"),
            dRofusComparison.Lt => ReturnComparison("lt"),
            dRofusComparison.Gt => ReturnComparison("gt"),
            dRofusComparison.Le => ReturnComparison("le"),
            dRofusComparison.Ge => ReturnComparison("ge"),
            dRofusComparison.In => $"{Field} in ({ReturnInValues(Value)})",
            dRofusComparison.Contains => $"contains({Field},{ConvertValue(Value)})",
            dRofusComparison.StartsWith => $"startswith({Field},{ConvertValue(Value)})",
            dRofusComparison.EndsWith => $"endswith({Field},{ConvertValue(Value)})",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string ReturnComparison(string comparison)
    {
        return $"{Field} {comparison} {ConvertValue(Value)}";
    }

    private string ReturnInValues(object? value)
    {
        if (value is not IEnumerable enumerable)
            return ConvertValue(value);

        var results = new List<string>();
        results.AddRange(from object? item in enumerable select ConvertValue(item));

        return string.Join(",", results);
    }

    private static string ConvertValue(object? value)
    {
        if (value is null)
            return "null";

        return value switch
        {
            DateTime dateTime => ConvertValue(dateTime.ToString("yyyy-M-d")),
            string stringValue => $"'{stringValue}'",
            _ => value.ToString()
        };
    }
}