namespace dRofusClient.Filters;

public record AndFilter(ICollection<FilterItem> Filters)
{
    public AndFilter And(FilterItem filterItem)
    {
        And([filterItem]);
        return this;
    }

    public AndFilter And(IEnumerable<FilterItem> filters)
    {
        foreach (var filter in filters)
            Filters.Add(filter);

        return this;
    }
}