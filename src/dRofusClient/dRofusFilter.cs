namespace dRofusClient;

public static class dRofusFilter
{
    public static dRofusFilterItem Eq(string field, object value) => new(field, dRofusComparison.Eq, value);
    public static dRofusFilterItem Ne(string field, object value) => new(field, dRofusComparison.Ne, value);
    public static dRofusFilterItem Lt(string field, object value) => new(field, dRofusComparison.Lt, value);
    public static dRofusFilterItem Gt(string field, object value) => new(field, dRofusComparison.Gt, value);
    public static dRofusFilterItem Le(string field, object value) => new(field, dRofusComparison.Le, value);
    public static dRofusFilterItem Ge(string field, object value) => new(field, dRofusComparison.Ge, value);
    public static dRofusFilterItem Contains(string field, object value) => new(field, dRofusComparison.Contains, value);
    public static dRofusFilterItem StartsWith(string field, object value) => new(field, dRofusComparison.StartsWith, value);
    public static dRofusFilterItem EndsWith(string field, object value) => new(field, dRofusComparison.EndsWith, value);
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

        var result = new List<dRofusFilterItem>();

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

    public static dRofusFilterItem In(string field, IEnumerable<object> values) => new(field, dRofusComparison.In, values);

    public static dRofusAndFilter And(ICollection<dRofusFilterItem> filters) => new(filters);
    public static dRofusAndFilter And(dRofusFilterItem filter1, dRofusFilterItem filter2) => And([filter1, filter2]);
    public static dRofusAndFilter And(dRofusFilterItem filter1, dRofusFilterItem filter2, dRofusFilterItem filter3) => And([filter1, filter2, filter3]);
    public static dRofusAndFilter And(dRofusFilterItem filter1, dRofusFilterItem filter2, dRofusFilterItem filter3, dRofusFilterItem filter4) => And([filter1, filter2, filter3, filter4]);
    public static dRofusAndFilter And(dRofusFilterItem filter1, dRofusFilterItem filter2, dRofusFilterItem filter3, dRofusFilterItem filter4, dRofusFilterItem filter5) => And([filter1, filter2, filter3, filter4, filter5]);

    public static dRofusAndFilter GtAndLt(int gt, string field, int lt)
        => new([Gt(field, gt), Lt(field, lt)]);

    public static dRofusFilterItem IsEmpty(string field)
        => new(field, dRofusComparison.Eq, null);

    public static dRofusFilterItem IsNotEmpty(string field)
        => new(field, dRofusComparison.Ne, null);
}