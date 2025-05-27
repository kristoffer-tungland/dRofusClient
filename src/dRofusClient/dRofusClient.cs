using System.Text;

namespace dRofusClient;

[GenerateInterface]
internal sealed class dRofusClient : IdRofusClient
{
    private readonly HttpClient _httpClient;
    private string? _database;
    private string? _projectId;
    private ModernLoginResult? _modernLoginResult;
    private readonly ILoginPromptHandler _loginPromptHandler;

    /// <summary>
    /// Exposes the internal HttpClient for advanced scenarios.
    /// </summary>
    public HttpClient HttpClient => _httpClient;

    // Optionally, expose a method to send custom requests
    public Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return _httpClient.SendAsync(request, cancellationToken);
    }

    public dRofusClient(HttpClient httpClient, ILoginPromptHandler loginPromptHandler)
    {
        _httpClient = httpClient;
        _loginPromptHandler = loginPromptHandler;
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public string? GetBaseUrl()
    {
        return _httpClient.BaseAddress?.ToString();
    }

    public void Setup(dRofusConnectionArgs args)
    {
        UpdateBaseAddress(args.BaseUrl);

        if (args is BasicConnectionArgs basic)
            UpdateAuthentication(basic.AuthenticationHeader);

        else if (args is ModernConnectionArgs modernLoginResult)
            UpdateAuthentication(modernLoginResult.LoginResult);

        _database = args.Database;
        _projectId = args.ProjectId;
    }

    private void UpdateBaseAddress(string baseUrl)
    {
        var newBaseUri = new Uri(baseUrl);
        if (_httpClient.BaseAddress == null || _httpClient.BaseAddress != newBaseUri)
        {
            _httpClient.BaseAddress = newBaseUri;
            // Optionally clear state or log
        }
    }

    public void UpdateAuthentication(string authenticationHeader)
    {
        _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authenticationHeader);
    }

    public void UpdateAuthentication(ModernLoginResult modernLoginResult)
    {
        _modernLoginResult = modernLoginResult;
        _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(modernLoginResult.TokenType + " " + modernLoginResult.AccessToken);
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
        catch (HttpRequestException)
        {
            await HandleLoginPromptAsync(cancellationToken);
            await Login(cancellationToken);
            return;
        }

        if (project is null)
            throw new dRofusClientLoginException("Logged in to dRofus, but failed to get any response");
    }

    private async Task HandleLoginPromptAsync(CancellationToken cancellationToken)
    {
        await _loginPromptHandler.Handle(this, cancellationToken);
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

    public void ClearAuthentication()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public void Logout()
    {
        _database = null;
        _projectId = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public (string database, string projectId) GetDatabaseAndProjectId()
    {
        if (_database is null)
            throw new dRofusClientLoginException("No database provided, please login");

        if (_projectId is null)
            throw new dRofusClientLoginException("No project id provided, please login");

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

        return await response.Content.ReadFromJsonAsync<TResult>(cancellationToken) ??
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
        var items = await response.Content.ReadFromJsonAsync<List<TResult>>(cancellationToken)
            ?? throw new NullReferenceException("Failed to read content from response.");

        if (options is dRofusListOptions listOptions && listOptions.GetNextItems())
            await GetNextItems(method, response, items, cancellationToken);

        return items;
    }

    private async Task GetNextItems<TResult>(HttpMethod method, HttpResponseMessage response, List<TResult> items, CancellationToken cancellationToken) where TResult : dRofusDto
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
        var nextItems = await nextResponse.Content.ReadFromJsonAsync<List<TResult>>(cancellationToken)
            ?? throw new NullReferenceException("Failed to read content from response.");

        items.AddRange(nextItems);

        await GetNextItems(method, nextResponse, items, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendResponse(
        HttpMethod method,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
        )
    {
        HttpRequestMessage? request;

        try
        {
            request = BuildRequest(method, route, options);
        }
        catch (dRofusClientLoginException)
        {
            await HandleLoginPromptAsync(cancellationToken);
            request = BuildRequest(method, route, options);
        }

        try
        {
            if (_modernLoginResult is not null)
                await RefreshTokenIfNeededAsync(cancellationToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            response.EnsureSuccessStatusCode();
            return response;
        }
        catch (HttpRequestException requestException)
        {
            if (requestException.Message.Contains(((int)System.Net.HttpStatusCode.Unauthorized).ToString()))
            {
                await HandleLoginPromptAsync(cancellationToken);
                request = BuildRequest(method, route, options);

                var response = await _httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                return response;
            }
            throw;
        }
    }

    private async Task RefreshTokenIfNeededAsync(CancellationToken cancellationToken)
    {
        if (_modernLoginResult is null || string.IsNullOrEmpty(_modernLoginResult.RefreshToken))
            return;

        if (_modernLoginResult.IsTokenAboutToExpire())
        {
            if (_loginPromptHandler is not ModernPromptHandler modernPromtHandler)
                throw new dRofusClientLoginException("Modern login handler does not support token refresh.");

            var result = await modernPromtHandler.HandleRefreshToken(this, _modernLoginResult.Server, _modernLoginResult.RefreshToken, cancellationToken);
            
            UpdateAuthentication(result);
        }
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