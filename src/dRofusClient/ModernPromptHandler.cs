using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;

namespace dRofusClient;

public class ModernPromptHandler : ILoginPromptHandler
{
    public async Task Handle(IdRofusClient client, CancellationToken cancellationToken)
    {
        var authority = Environment.GetEnvironmentVariable("OAUTH2_AUTHORITY");
        var clientId = Environment.GetEnvironmentVariable("OAUTH2_CLIENTID");
        var clientSecret = Environment.GetEnvironmentVariable("OAUTH2_CLIENTSECRET");
        var scope = Environment.GetEnvironmentVariable("OAUTH2_SCOPE");
        var redirectUri = Environment.GetEnvironmentVariable("OAUTH2_REDIRECTURI");

        if (string.IsNullOrWhiteSpace(authority) ||
            string.IsNullOrWhiteSpace(clientId) ||
            string.IsNullOrWhiteSpace(clientSecret) ||
            string.IsNullOrWhiteSpace(scope))
        {
            throw new InvalidOperationException("OAuth2 environment variables are not set.");
        }

        string? db = null, pr = null;
        try
        {
            var (dbVal, prVal) = client.GetDatabaseAndProjectId();
            db = dbVal;
            pr = prVal;
        }
        catch (Exception)
        {
            // Optionally handle missing db/pr
        }

        var browser = new SystemBrowser(new Uri(redirectUri).Port);

        var options = new OidcClientOptions
        {
            Authority = authority,
            ClientId = clientId,
            ClientSecret = clientSecret,
            Scope = scope,
            RedirectUri = redirectUri,
            Browser = browser,
            LoadProfile = false
        };

        var oidcClient = new OidcClient(options);

        var parameters = new Duende.IdentityModel.Client.Parameters();
        if (!string.IsNullOrEmpty(db)) parameters.Add("db", db);
        if (!string.IsNullOrEmpty(pr)) parameters.Add("pr", pr);

        var loginState = await oidcClient.PrepareLoginAsync(parameters, cancellationToken);

        // Use the browser instance directly, not loginState.Browser
        var browserResult = await browser.InvokeAsync(
            new BrowserOptions(loginState.StartUrl,redirectUri),
            cancellationToken
        );

        var result = await oidcClient.ProcessResponseAsync(browserResult.Response, loginState, parameters, cancellationToken);

        if (result.IsError)
            throw new Exception(result.Error + "\n" + result.ErrorDescription);

        client.UpdateAuthentication($"Bearer {result.AccessToken}");
    }
}