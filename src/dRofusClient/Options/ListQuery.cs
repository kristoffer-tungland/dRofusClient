namespace dRofusClient.Options;



public record ListQuery : ItemQuery
{
    internal readonly List<string> _orderBy = [];
    internal dRofusOrderBy _orderByDirection = dRofusOrderBy.Ascending;
    internal readonly List<FilterItem> _comparisons = [];
    internal int? _top;
    internal int? _skip;

    private RequestParameter? GetOrderByParameter()
    {
        if (!_orderBy.Any())
            return null;

        return new RequestParameter("orderby", _orderBy.ToCommaSeparated() + " " + _orderByDirection.ToQuery());
    }

    private RequestParameter? GetComparisonParameter()
    {
        if (!_comparisons.Any())
            return null;

        var comparisons = string.Join(" and ", _comparisons);

        return new RequestParameter("filter", comparisons);
    }

    private RequestParameter? GetTopParameter()
    {
        return _top is null ? null : new RequestParameter("top", _top.ToString());
    }

    private RequestParameter? GetSkipParameter()
    {
        return _skip is null ? null : new RequestParameter("skip", _skip.ToString());
    }

    public override void AddParametersToRequest(List<RequestParameter> parameters)
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