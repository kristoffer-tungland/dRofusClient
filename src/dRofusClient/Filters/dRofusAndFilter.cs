namespace dRofusClient.Filters;

public record dRofusAndFilter(ICollection<dRofusFilterItem> Filters)
{
    public dRofusAndFilter And(dRofusFilterItem filterItem)
    {
        And([filterItem]);
        return this;
    }

    public dRofusAndFilter And(IEnumerable<dRofusFilterItem> filters)
    {
        foreach (var filter in filters)
            Filters.Add(filter);

        return this;
    }
}