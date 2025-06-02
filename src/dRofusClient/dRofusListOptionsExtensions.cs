// ReSharper disable InconsistentNaming
namespace dRofusClient;

public static class dRofusListOptionsExtensions
{
    private static TOption OrderBy<TOption>(this TOption option, string field, dRofusOrderBy orderBy)
        where TOption : dRofusListOptions
    {
        option._orderBy.Add(field.ToSnakeCase());
        option._orderByDirection = orderBy;
        return option;
    }

    private static TOption OrderBy<TOption>(this TOption option, IEnumerable<string> fields, dRofusOrderBy orderBy)
        where TOption : dRofusListOptions
    {
        option._orderBy.AddRange(fields.Select(x => x.ToSnakeCase()));
        option._orderByDirection = orderBy;
        return option;
    }

    public static TOption OrderBy<TOption>(this TOption option, string field)
        where TOption : dRofusListOptions
    {
        return option.OrderBy(field, dRofusOrderBy.Ascending);
    }

    public static TOption OrderBy<TOption>(this TOption option, IEnumerable<string> fields)
        where TOption : dRofusListOptions
    {
        return option.OrderBy(fields, dRofusOrderBy.Ascending);
    }

    public static TOption OrderByDescending<TOption>(this TOption option, string field)
        where TOption : dRofusListOptions
    {
        return option.OrderBy(field, dRofusOrderBy.Descending);
    }

    public static TOption OrderByDescending<TOption>(this TOption option, IEnumerable<string> fields)
        where TOption : dRofusListOptions
    {
        return option.OrderBy(fields, dRofusOrderBy.Descending);
    }

    public static TOption Filter<TOption>(this TOption option, dRofusFilterItem dRofusAndFilter)
        where TOption : dRofusListOptions
    {
        option._comparisons.Add(dRofusAndFilter);
        return option;
    }

    public static TOption Filter<TOption>(this TOption option, dRofusAndFilter dRofusAndFilter)
        where TOption : dRofusListOptions
    {
        option._comparisons.AddRange(dRofusAndFilter.Filters);
        return option;
    }

    public static TOption Top<TOption>(this TOption option, int top)
        where TOption : dRofusListOptions
    {
        option._top = top;
        return option;
    }
    public static TOption Skip<TOption>(this TOption option, int skip)
        where TOption : dRofusListOptions
    {
        option._skip = skip;
        return option;
    }
}