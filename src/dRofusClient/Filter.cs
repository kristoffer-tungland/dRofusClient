

namespace dRofusClient;

public static class Filter
{
    public static FilterItem Eq(string field, object value) => new(field, Comparison.Eq, value);
    public static FilterItem Ne(string field, object value) => new(field, Comparison.Ne, value);

    public static FilterItem Lt(string field, object value) => new(field, Comparison.Lt, value);
    public static FilterItem Gt(string field, object value) => new(field, Comparison.Gt, value);
    public static FilterItem Le(string field, object value) => new(field, Comparison.Le, value);
    public static FilterItem Ge(string field, object value) => new(field, Comparison.Ge, value);
    public static FilterItem Contains(string field, object value) => new(field, Comparison.Contains, value);
    public static FilterItem StartsWith(string field, object value) => new(field, Comparison.StartsWith, value);
    public static FilterItem EndsWith(string field, object value) => new(field, Comparison.EndsWith, value);

    public static FilterItem Empty(string field) => new(field, Comparison.Eq, null);
    public static FilterItem NotEmpty(string field) => new(field, Comparison.Ne, null);

    public static AndFilter Wildcard(string field, string value) => Wildcard(field, value.Split('*'));

    private static AndFilter Wildcard(string field, IReadOnlyList<string> strings)
    {
        var last = strings.Count;

        switch (last)
        {
            case < 1:
                return And([]);
            case 1:
                return And([Contains(field, strings.First())]);
        }

        var result = new List<FilterItem>();

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

    public static FilterItem In<T>(string field, IEnumerable<T> values) => new(field, Comparison.In, values);
    public static FilterItem In<T>(string field, params T[] values) => new(field, Comparison.In, values);

    public static AndFilter And(ICollection<FilterItem> filters) => new(filters);
    public static AndFilter And(IEnumerable<FilterItem> filters) => new([.. filters]);
    public static AndFilter And(params FilterItem[] filters) => new(filters);

    public static AndFilter GtAndLt(int gt, string field, int lt)
        => new([Gt(field, gt), Lt(field, lt)]);

    public static FilterItem IsEmpty(string field)
        => new(field, Comparison.Eq, null);

    public static FilterItem IsNotEmpty(string field)
        => new(field, Comparison.Ne, null);

}