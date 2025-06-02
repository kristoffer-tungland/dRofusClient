using Duende.IdentityModel.Client;
using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using Duende.IdentityModel.OidcClient.Results;
using Microsoft.Extensions.Logging;

namespace dRofusClient;

public record ModernLoginOptions(
    string ClientId,
    string ClientSecret,
    string Scope,
    string RedirectUri
);

public record ModernConnectionArgs(
    string BaseUrl,
    string Database,
    string ProjectId,
    ModernLoginResult LoginResult
) : dRofusConnectionArgs(BaseUrl, Database, ProjectId)
{
    public static ModernConnectionArgs Create(dRofusServer server, string database, string projectId,
        ModernLoginResult loginResult)
    {
        if (string.IsNullOrEmpty(loginResult.AccessToken))
            throw new Exception("Supplied access token is empty.");

        return new ModernConnectionArgs(server.Adress, database, projectId, loginResult);
    }
};

public record ModernLoginResult(string AccessToken, string RefreshToken, string IdToken, DateTimeOffset ExpiresAt, dRofusServer Server)
{
    public string TokenType => "Bearer"; // Assuming Bearer token type, adjust if necessary

    public static ModernLoginResult FromLoginResult(LoginResult result, dRofusServer server)
    {
        return new ModernLoginResult(
            result.AccessToken,
            result.RefreshToken,
            result.IdentityToken,
            result.AccessTokenExpiration,
            Server: server
        );
    }

    public static ModernLoginResult FromLoginResult(RefreshTokenResult result, dRofusServer server)
    {
        return new ModernLoginResult(
            result.AccessToken,
            result.RefreshToken,
            result.IdentityToken,
            result.AccessTokenExpiration,
            Server: server
        );
    }

    public bool IsTokenAboutToExpire()
    {
        // Check if the token is about to expire within the next 5 minutes
        return ExpiresAt.UtcDateTime <= DateTimeOffset.UtcNow.AddMinutes(5);
    }
}

public class ModernPromptHandler(ModernLoginOptions modernLoginOptions, ILogger? logger = default) : ILoginPromptHandler
{
    public async Task Handle(IdRofusClient client, CancellationToken cancellationToken)
    {
        string? db = null, pr = null;
        try
        {
            var (dbVal, prVal) = client.GetDatabaseAndProjectId();
            db = dbVal;
            pr = prVal;
        }
        catch (dRofusClientLoginException e)
        {
            logger?.LogWarning(e, "Failed to retrieve database and project ID from the client. The client must be configured with a database and project ID to use modern login.");
            throw new dRofusClientLoginException("Failed to retrieve database and project ID from the client.");
        }

        var baseUrl = client.GetBaseUrl();
        var server = dRofusServer.FromBaseUrl(baseUrl!);

        var result = await HandleOidcAuthenticationAsync(server, db, pr, cancellationToken);
        client.UpdateAuthentication(result);
    }

    public async Task<ModernLoginResult> HandleOidcAuthenticationAsync(
        dRofusServer server, string db, string pr, CancellationToken cancellationToken = default)
    {
        var authority = server.Authority;
        var browser = new SystemBrowser(new Uri(modernLoginOptions.RedirectUri).Port);

        var discoveryDocument = await GetDiscoveryDocument(new HttpClient(), server);

        var options = new OidcClientOptions
        {
            Authority = authority,
            ClientId = modernLoginOptions.ClientId,
            ClientSecret = modernLoginOptions.ClientSecret,
            Scope = modernLoginOptions.Scope,
            RedirectUri = modernLoginOptions.RedirectUri,
            Browser = browser,
            LoadProfile = false
        };

        var oidcClient = new OidcClient(options);

        var parameters = new Duende.IdentityModel.Client.Parameters();
        if (!string.IsNullOrEmpty(db)) parameters.Add("db", db!);
        if (!string.IsNullOrEmpty(pr)) parameters.Add("pr", pr!);

        var loginState = await oidcClient.PrepareLoginAsync(parameters, cancellationToken);

        // Use the browser instance directly, not loginState.Browser
        var browserResult = await browser.InvokeAsync(
            new BrowserOptions(loginState.StartUrl, options.RedirectUri),
            cancellationToken
        );

        var result = await oidcClient.ProcessResponseAsync(browserResult.Response, loginState, parameters, cancellationToken);

        if (result.IsError)
            throw new dRofusClientModernLoginException(result.Error)
            { ErrorDescription = result.ErrorDescription };

        return ModernLoginResult.FromLoginResult(result, server);
    }

    public async Task<ModernLoginResult> HandleRefreshToken(IdRofusClient client, dRofusServer server, string refreshToken, CancellationToken cancellationToken)
    {
        var discoveryDocument = await GetDiscoveryDocument(client.HttpClient, server);
        var modernClient = client.HttpClient;

        var options = new OidcClientOptions
        {
            Authority = server.Authority,
            ClientId = modernLoginOptions.ClientId,
            ClientSecret = modernLoginOptions.ClientSecret,
            Scope = modernLoginOptions.Scope,
            RedirectUri = modernLoginOptions.RedirectUri,
            Browser = new SystemBrowser(new Uri(modernLoginOptions.RedirectUri).Port),
            LoadProfile = false
        };
        var oidcClient = new OidcClient(options);
        var result = await oidcClient.RefreshTokenAsync(refreshToken, scope: modernLoginOptions.Scope, cancellationToken: cancellationToken);
        
        if (result.IsError)
            throw new dRofusClientModernLoginException(result.Error)
            { ErrorDescription = result.ErrorDescription };
        
        return ModernLoginResult.FromLoginResult(result, server);
    }

    private async Task<DiscoveryDocumentResponse> GetDiscoveryDocument(HttpClient client, dRofusServer server)
    {
        var discoveryDocument = await client.GetDiscoveryDocumentAsync(server.Authority);
        
        if (discoveryDocument.IsError)
            throw new dRofusClientLoginException(discoveryDocument.Error ?? "Failed to retrieve discovery document.");

        return discoveryDocument;
    }
}