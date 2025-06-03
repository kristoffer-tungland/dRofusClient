using System.Text;
using System.IO;

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

        if (options is dRofusOptionsBodyBase bodyOptions)
            request.Content = new StringContent(bodyOptions.GetBody(), Encoding.UTF8, bodyOptions.Accept);

        return request;
    }

    /// <summary>
    /// Gets binary data from the specified route
    /// </summary>
    /// <param name="route">API route</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary data as byte array</returns>
    public async Task<byte[]> GetBytesAsync(string route, CancellationToken cancellationToken = default)
    {
        var response = await SendResponse(HttpMethod.Get, route, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    /// <summary>
    /// Gets a stream from the specified route
    /// </summary>
    /// <param name="route">API route</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Data stream</returns>
    public async Task<Stream> GetStreamAsync(string route, CancellationToken cancellationToken = default)
    {
        var response = await SendResponse(HttpMethod.Get, route, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    /// <summary>
    /// Posts a file to the specified route
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="route">API route</param>
    /// <param name="options">File upload options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API response</returns>
    public async Task<TResult> PostFileAsync<TResult>(string route, object options, CancellationToken cancellationToken = default) where TResult : dRofusDto, new()
    {
        // Extract file information from the options object using reflection
        var fileContentProp = options.GetType().GetProperty("FileContent") ?? options.GetType().GetProperty("ImageContent");
        var fileNameProp = options.GetType().GetProperty("FileName");
        
        if (fileContentProp == null || fileNameProp == null)
            throw new ArgumentException("The options object must have FileContent/ImageContent and FileName properties", nameof(options));

        var fileContent = fileContentProp.GetValue(options) as Stream;
        var fileName = fileNameProp.GetValue(options) as string;

        if (fileContent == null || string.IsNullOrEmpty(fileName))
            throw new ArgumentException("FileContent/ImageContent and FileName must be provided", nameof(options));

        // Create form content
        using var content = new MultipartFormDataContent();
        
        // Add file content
        var fileStreamContent = new StreamContent(fileContent);
        content.Add(fileStreamContent, "file", fileName);
        
        // Add any additional properties as form fields
        foreach (var prop in options.GetType().GetProperties())
        {
            if (prop.Name != "FileContent" && prop.Name != "ImageContent" && prop.Name != "FileName")
            {
                var value = prop.GetValue(options)?.ToString();
                if (value != null)
                    content.Add(new StringContent(value), prop.Name);
            }
        }

        // Create request
        var url = route;
        if (!url.StartsWith("/api"))
        {
            var (database, projectId) = GetDatabaseAndProjectId();
            url = $"/api/{database}/{projectId}/" + url.TrimStart('/');
        }

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        // Send request
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        // Parse response
        return await response.Content.ReadFromJsonAsync<TResult>(cancellationToken) ??
               throw new NullReferenceException("Failed to read content from response.");
    }

    /// <summary>
    /// Deletes a resource (with a result DTO)
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="route">API route</param>
    /// <param name="options">Request options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public async Task<TResult> DeleteAsync<TResult>(string route, dRofusOptionsBase? options = default, CancellationToken cancellationToken = default) where TResult : dRofusDto, new()
    {
        return await SendAsync<TResult>(HttpMethod.Delete, route, options, cancellationToken);
    }

    /// <summary>
    /// Deletes a resource (with no return value)
    /// </summary>
    /// <param name="route">API route</param>
    /// <param name="options">Request options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the completion of the operation</returns>
    public async Task DeleteAsync(string route, dRofusOptionsBase? options = default, CancellationToken cancellationToken = default)
    {
        await SendAsync<dRofusBaseResponse>(HttpMethod.Delete, route, options, cancellationToken);
    }

    /// <summary>
    /// Base response type for void operations
    /// </summary>
    private record dRofusBaseResponse : dRofusDto { }
}