namespace dRofusClient.Options;

public record dRofusListOptions : dRofusFieldsOptions
{
    internal readonly List<string> _orderBy = [];
    internal dRofusOrderBy _orderByDirection = dRofusOrderBy.Ascending;
    internal readonly List<dRofusFilterItem> _comparisons = [];
    internal int? _top;
    internal int? _skip;

    dRofusRequestParameter? GetOrderByParameter()
    {
        if (!_orderBy.Any())
            return null;

        return new dRofusRequestParameter("orderby", _orderBy.ToCommaSeparated() + " " + _orderByDirection.ToQuery());
    }

    dRofusRequestParameter? GetComparisonParameter()
    {
        if (!_comparisons.Any())
            return null;

        var comparisons = string.Join(" and ", _comparisons);

        return new dRofusRequestParameter("filter", comparisons);
    }

    dRofusRequestParameter? GetTopParameter()
    {
        return _top is null ? null : new dRofusRequestParameter("top", _top.ToString());
    }

    dRofusRequestParameter? GetSkipParameter()
    {
        return _skip is null ? null : new dRofusRequestParameter("skip", _skip.ToString());
    }

    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
        base.AddParametersToRequest(parameters);
        parameters.AddIfNotNull(GetOrderByParameter());
        parameters.AddIfNotNull(GetComparisonParameter());
        parameters.AddIfNotNull(GetTopParameter());
        parameters.AddIfNotNull(GetSkipParameter());
    }

    public bool GetNextItems()
    {
        return _top is null;
    }
}