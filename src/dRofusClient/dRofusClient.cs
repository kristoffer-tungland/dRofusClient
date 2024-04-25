// ReSharper disable InconsistentNaming

using System.Text;

namespace dRofusClient;

[Register<IdRofusClient>, GenerateInterface]
internal sealed class dRofusClient : IdRofusClient
{
    readonly HttpClient _httpClient;
    string? _database;
    string? _projectId;
    
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

    public Task<TResult> SendAsync<TResult>(
        HttpMethod method,
        dRofusType dRofusType,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        ) where TResult : dRofusDto, new()
    {
        return SendAsync<TResult>(method, dRofusType.ToRequest(), options, cancellationToken);
    }

    public async Task<TResult> SendAsync<TResult>(
        HttpMethod method,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        ) where TResult : dRofusDto, new()
    {
        var response = await SendResponse(method, route, options, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (method == HttpMethod.Delete)
            return new TResult();

        return await response.Content.ReadFromJsonAsync<TResult>() ??
               throw new NullReferenceException("Failed to read content from response.");
    }

    public Task<List<TResult>> SendListAsync<TResult>(
        HttpMethod method,
        dRofusType dRofusType,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        ) where TResult : dRofusDto
    {
        return SendListAsync<TResult>(method, dRofusType.ToRequest(), options, cancellationToken);
    }

    public async Task<List<TResult>> SendListAsync<TResult>(
        HttpMethod method,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        ) where TResult : dRofusDto
    {
        var response = await SendResponse(method, route, options, cancellationToken);
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

        if (string.IsNullOrEmpty(nextLink)) 
            return;

        var request = new HttpRequestMessage(method, nextLink);
        var nextResponse = await _httpClient.SendAsync(request, cancellationToken);
        nextResponse.EnsureSuccessStatusCode();
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

        switch (options)
        {
            case dRofusBodyPatchOptions bodyPatchOptions:
            {
                var body = bodyPatchOptions.Body;
                request.Content = new StringContent(body, Encoding.UTF8, "application/merge-patch+json");
                break;
            }
            case dRofusBodyPostOptions bodyPostOptions:
            {
                var body = bodyPostOptions.Body;
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                break;
            }
        }

        return request;
    }
}