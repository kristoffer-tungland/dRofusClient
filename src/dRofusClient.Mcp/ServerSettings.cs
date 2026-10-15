namespace dRofusClient.Mcp;

public sealed class ServerSettings
{
    public required string BaseUrl { get; init; }
    public required string Database { get; init; }
    public required string ProjectId { get; init; }
    public required string Username { private get; init; }
    public required string Password { private get; init; }
    public bool EnableWrites { get; init; }

    public ProjectContext Context => new(BaseUrl, Database, ProjectId);

    public static ServerSettings FromEnvironment(Func<string, string?> get) =>
        FromEnvironment(get, new WindowsCredentialPasswordStore());

    public static ServerSettings FromEnvironment(Func<string, string?> get, IWindowsCredentialPasswordStore credentialStore)
    {
        string Required(string name) => !string.IsNullOrWhiteSpace(get(name))
            ? get(name)! : throw new ArgumentException("Missing required environment variable.");
        var baseUrl = Required("DROFUS_BASE_URL");
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(uri.UserInfo) || uri.AbsolutePath != "/" ||
            !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
            throw new ArgumentException("An HTTPS server origin is required.");
        var database = Required("DROFUS_DATABASE");
        var project = Required("DROFUS_PROJECT_ID");
        if (!SafeSegment(database) || !SafeSegment(project))
            throw new ArgumentException("Invalid database or project.");
        var writes = get("DROFUS_ENABLE_WRITES");
        if (writes is not null && !bool.TryParse(writes, out _))
            throw new ArgumentException("Enable writes must be true or false.");
        var useWindowsCredentials = get("DROFUS_USE_WINDOWS_CREDENTIALS");
        if (useWindowsCredentials is not null && !bool.TryParse(useWindowsCredentials, out _))
            throw new ArgumentException("Use Windows credentials must be true or false.");
        var username = Required("DROFUS_USERNAME");
        var password = get("DROFUS_PASSWORD");
        if (string.IsNullOrWhiteSpace(password) && bool.TryParse(useWindowsCredentials, out var useStore) && useStore)
        {
            try
            {
                password = credentialStore.ReadPassword(uri.GetLeftPart(UriPartial.Authority), username);
            }
            catch (Exception)
            {
                // Store exceptions may contain credential targets or account information.
                throw new ArgumentException("Windows credential lookup failed. Check platform, account and saved credential.");
            }
        }
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("No password available. Supply DROFUS_PASSWORD or configure Windows credentials.");
        return new ServerSettings
        {
            BaseUrl = uri.GetLeftPart(UriPartial.Authority),
            Database = database,
            ProjectId = project,
            Username = username,
            Password = password,
            EnableWrites = bool.TryParse(writes, out var enabled) && enabled
        };
    }

    public IdRofusClient CreateClient()
    {
        var connection = dRofusConnectionArgs.Create(BaseUrl, Database, ProjectId, Username, Password);
        if (new Uri(connection.BaseUrl) != new Uri(BaseUrl))
            throw new ArgumentException("Use the canonical dRofus API server origin.");
        var client = new dRofusClientFactory().Create(connection);
        client.HttpClient.Timeout = TimeSpan.FromSeconds(60);
        return client;
    }

    private static bool SafeSegment(string value) =>
        value.Length <= 128 && value.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-');
}

public sealed record ProjectContext(string Server, string Database, string ProjectId);
