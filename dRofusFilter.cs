using System.Collections;
using System.Linq;

namespace dRofusClient;

public record dRofusFilter(string Field, dRofusComparison Comparison, object? Value)
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

    string ReturnComparison(string comparison)
    {
        return $"{Field} {comparison} {ConvertValue(Value)}";
    }

    string ReturnInValues(object? value)
    {
        if (value is not IEnumerable enumerable)
            return ConvertValue(value);

        var results = new List<string>();
        results.AddRange(from object? item in enumerable select ConvertValue(item));

        return string.Join(",", results);
    }

    static string ConvertValue(object? value)
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

    public static dRofusFilter Eq(string field, object value) => new(field, dRofusComparison.Eq, value);
    public static dRofusFilter Ne(string field, object value) => new(field, dRofusComparison.Ne, value);
    public static dRofusFilter Lt(string field, object value) => new(field, dRofusComparison.Lt, value);
    public static dRofusFilter Gt(string field, object value) => new(field, dRofusComparison.Gt, value);
    public static dRofusFilter Le(string field, object value) => new(field, dRofusComparison.Le, value);
    public static dRofusFilter Ge(string field, object value) => new(field, dRofusComparison.Ge, value);
    public static dRofusFilter Contains(string field, object value) => new(field, dRofusComparison.Contains, value);
    public static dRofusFilter StartsWith(string field, object value) => new(field, dRofusComparison.StartsWith, value);
    public static dRofusFilter EndsWith(string field, object value) => new(field, dRofusComparison.EndsWith, value);
    public static dRofusAndFilter Wildcard(string field, string value) => Wildcard(field, value.Split('*'));

    static dRofusAndFilter Wildcard(string field, IReadOnlyList<string> strings)
    {
        var last = strings.Count;

        switch (last)
        {
            case < 1:
                return And([]);
            case 1:
                return And([Contains(field, strings.First())]);
        }

        var result = new List<dRofusFilter>();

        for (var i = 0; i < last; i++)
        {
            var value = strings[i];

            if (i == 0)
                result.Add(StartsWith(field, value));
            else if (i == last - 1)
                result.Add(EndsWith(field, value));
            else
                result.Add(Contains(field, value));
        }

        return And(result);
    }

    public static dRofusFilter In(string field, IEnumerable<object> values) => new(field, dRofusComparison.In, values);

    public static dRofusAndFilter And(ICollection<dRofusFilter> filters) => new(filters);
    public static dRofusAndFilter And(dRofusFilter filter1, dRofusFilter filter2) => And([filter1, filter2]);
    public static dRofusAndFilter And(dRofusFilter filter1, dRofusFilter filter2, dRofusFilter filter3) => And([filter1, filter2, filter3]);
    public static dRofusAndFilter And(dRofusFilter filter1, dRofusFilter filter2, dRofusFilter filter3, dRofusFilter filter4) => And([filter1, filter2, filter3, filter4]);
    public static dRofusAndFilter And(dRofusFilter filter1, dRofusFilter filter2, dRofusFilter filter3, dRofusFilter filter4, dRofusFilter filter5) => And([filter1, filter2, filter3, filter4, filter5]);

    public static dRofusAndFilter GtAndLt(int gt, string field, int lt)
        => new([Gt(field, gt), Lt(field, lt)]);

    public static dRofusFilter IsEmpty(string field)
        => new(field, dRofusComparison.Eq, null);

    public static dRofusFilter IsNotEmpty(string field)
        => new(field, dRofusComparison.Ne, null);
}

public record dRofusAndFilter(ICollection<dRofusFilter> Filters)
{
    public dRofusAndFilter And(dRofusFilter filter)
    {
        And([filter]);
        return this;
    }

    public dRofusAndFilter And(IEnumerable<dRofusFilter> filters)
    {
        foreach (var filter in filters)
            Filters.Add(filter);

        return this;
    }
}
