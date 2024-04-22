// ReSharper disable InconsistentNaming

using System.Linq;
using dRofusClient.Bases;
using dRofusClient.Projects;
using dRofusClient.Exceptions;
using dRofusClient.Parameters;
using dRofusClient.PropertyMeta;
using System.Threading;

namespace dRofusClient;

[Register<IdRofusClient>, GenerateInterface]
internal class dRofusClient : IdRofusClient
{
    readonly HttpClient _httpClient;
    string? _database;
    string? _projectId;

    public dRofusClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public dRofusClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public string GetBaseUrl()
    {
        return _httpClient.BaseAddress?.ToString() ?? throw new Exception("No base URL provided.");
    }

    public void Setup(dRofusConnectionArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.AuthenticationHeader))
            throw new dRofusClientCreateException("No authentication provided.");

        if (string.IsNullOrWhiteSpace(args.Database))
            throw new dRofusClientCreateException("No dRofus database provided.");

        if (string.IsNullOrWhiteSpace(args.ProjectId))
            throw new dRofusClientCreateException("No dRofus project id provided.");

        _httpClient.BaseAddress = new Uri(args.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(args.AuthenticationHeader);

        _database = args.Database;
        _projectId = args.ProjectId;
    }

    public Task Login(dRofusConnectionArgs args, CancellationToken cancellationToken = default)
    {
        Setup(args);
        return Login(cancellationToken);
    }

    public async Task Login(CancellationToken cancellationToken = default)
    {
        dRofusProject? project;

        try
        {
            project = await this.GetProjectAsync(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException requestException)
        {
            throw new dRofusClientLoginException($"Failed to login to dRofus: {requestException.Message}");
        }

        if (project is null)
            throw new dRofusClientLoginException("Logged in to dRofus, but failed to get any response");
    }

    public async Task<bool> IsLoggedIn()
    {
        try
        {
            await this.GetProjectAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Logout()
    {
        _database = null;
        _projectId = null;
        _httpClient.BaseAddress = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public (string database, string projectId) GetDatabaseAndProjectId()
    {
        if (_database is null)
            throw new Exception("No database provided, please login");

        if (_projectId is null)
            throw new Exception("No project id provided, please login");

        return (_database, _projectId);
    }

    public async Task<TResult> SendAsync<TResult>(
        HttpMethod method,
        dRofusType dRofusType,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        ) where TResult : dRofusDto
    {
        var response = await SendResponse(method, dRofusType.ToRequest(), options, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResult>() ??
               throw new NullReferenceException("Failed to read content from response.");
    }

    public async Task<List<TResult>> SendListAsync<TResult>(
        HttpMethod method,
        dRofusType dRofusType,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        ) where TResult : dRofusDto
    {
        var response = await SendResponse(method, dRofusType.ToRequest(), options, cancellationToken);
        var items = await response.Content.ReadFromJsonAsync<List<TResult>>() ?? throw new NullReferenceException("Failed to read content from response.");
        
        if (options is dRofusListOptions listOptions && listOptions.GetNextItems())
            await GetNextItems(method, response, items, cancellationToken);

        return items;
    }

    async Task GetNextItems<TResult>(HttpMethod method, HttpResponseMessage response, List<TResult> items, CancellationToken cancellationToken) where TResult : dRofusDto
    {
        if (!response.Headers.Contains("Link"))
            return;

        var link = response.Headers.GetValues("Link").First();
        var nextLink = link.Split(';')[0].Trim('<', '>');
        var request = new HttpRequestMessage(method, nextLink);
        var nextResponse = await _httpClient.SendAsync(request, cancellationToken);
        var nextItems = await nextResponse.Content.ReadFromJsonAsync<List<TResult>>() ?? throw new NullReferenceException("Failed to read content from response.");
        items.AddRange(nextItems);

        await GetNextItems(method, nextResponse, items, cancellationToken);
    }

    async Task<HttpResponseMessage> SendResponse(
        HttpMethod method,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        )
    {
        var request = BuildRequest(method, route, options);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response;
    }

    internal HttpRequestMessage BuildRequest(HttpMethod method, string route, dRofusOptionsBase? options)
    {
        var url = route;

        if (!url.StartsWith("/api"))
        {
            var (database, projectId) = GetDatabaseAndProjectId();
            url = $"/api/{database}/{projectId}/" + url.TrimStart('/');
        }

        if (options?.GetParameters() is { } parameters)
        {
            url += "?" + parameters;
        }

        var request = new HttpRequestMessage(method, url);
        return request;
    }
}

public record dRofusFieldsOptions : dRofusOptionsBase
{
    internal readonly List<string> _fieldsToSelect = [];

    dRofusRequestParameter? GetSelectParameters()
    {
        return !_fieldsToSelect.Any() ? null : new dRofusRequestParameter("select", _fieldsToSelect);
    }

    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
        parameters.AddIfNotNull(GetSelectParameters());
    }
}

public record dRofusListOptions : dRofusFieldsOptions
{
    internal readonly List<string> _orderBy = [];
    internal dRofusOrderBy _orderByDirection = dRofusOrderBy.Ascending;
    internal readonly List<dRofusFilter> _comparisons = [];
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

public static class dRofusPropertyToFieldExtensions
{
    public static string ToLowerUnderscore(this string field)
    {
        return field.ToLower();
    }
}

public static class dRofusListOptionsExtensions
{
    static TOption OrderBy<TOption>(this TOption option, string field, dRofusOrderBy orderBy)
        where TOption : dRofusListOptions
    {
        option._orderBy.Add(field.ToLowerUnderscore());
        option._orderByDirection = orderBy;
        return option;
    }

    static TOption OrderBy<TOption>(this TOption option, IEnumerable<string> fields, dRofusOrderBy orderBy)
        where TOption : dRofusListOptions
    {
        option._orderBy.AddRange(fields.Select(x => x.ToLowerUnderscore()));
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

    public static TOption Filter<TOption>(this TOption option, dRofusFilter dRofusFilter)
        where TOption : dRofusListOptions
    {
        option._comparisons.Add(dRofusFilter);
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

public static class dRofusFieldsOptionsExtensions
{
    public static TOption Select<TOption>(this TOption options, string field)
        where TOption : dRofusFieldsOptions
    {
        return options.Select([field]);
    }

    public static TOption Select<TOption>(this TOption options, IEnumerable<string> fields)
        where TOption : dRofusFieldsOptions
    {
        options._fieldsToSelect.AddRange(fields.Select(x => x.ToLowerUnderscore()));
        return options;
    }
}

public static class dRofusOptions
{
    public static dRofusFieldsOptions Field() => new();
    public static dRofusListOptions List() => new();
    public static dRofusPropertyMetaOptions PropertyMeta(int depth = 0) => new(depth);
}