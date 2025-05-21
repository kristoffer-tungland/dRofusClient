using Duende.IdentityModel.Client;

namespace dRofusClient;

public class ModernPromptHandler : ILoginPromptHandler
{
    public async Task Handle(IdRofusClient client, CancellationToken cancellationToken)
    {
        var authority = Environment.GetEnvironmentVariable("OAUTH2_AUTHORITY");
        var clientId = Environment.GetEnvironmentVariable("OAUTH2_CLIENTID");
        var clientSecret = Environment.GetEnvironmentVariable("OAUTH2_CLIENTSECRET");
        var scope = Environment.GetEnvironmentVariable("OAUTH2_SCOPE");

        if (string.IsNullOrWhiteSpace(authority) ||
            string.IsNullOrWhiteSpace(clientId) ||
            string.IsNullOrWhiteSpace(clientSecret) ||
            string.IsNullOrWhiteSpace(scope))
        {
            throw new InvalidOperationException("OAuth2 environment variables are not set.");
        }

        var httpClient = client.HttpClient;

        var disco = await httpClient.GetDiscoveryDocumentAsync(authority, cancellationToken);
        if (disco.IsError)
            throw new System.Exception($"Discovery failed: {disco.Error}");

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = clientId,
            ClientSecret = clientSecret,
            Scope = scope,
        }, cancellationToken);

        if (tokenResponse.IsError)
            throw new System.Exception($"Token request failed: {tokenResponse.Error}");

        client.UpdateAuthentication($"Bearer {tokenResponse.AccessToken}");
    }
}