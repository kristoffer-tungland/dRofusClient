using System.Text;

namespace dRofusClient;

public record BasicConnectionArgs(string BaseUrl, string Database, string ProjectId, string Username, string Password) : dRofusConnectionArgs(BaseUrl, Database, ProjectId)
{
    public string AuthenticationHeader => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"))}";
}

public record dRofusConnectionArgs(string BaseUrl, string Database, string ProjectId)
{
    public string BaseUrl { get; } = NormalizeServerAddress(BaseUrl);

    private static string NormalizeServerAddress(string baseUrl)
    {
        if (baseUrl.Equals(dRofusServers.GetDefaultServer(), StringComparison.OrdinalIgnoreCase))
            return dRofusServers.GetNoServer();

        if (baseUrl.StartsWith("https://api.", StringComparison.OrdinalIgnoreCase) == false)
            baseUrl = "https://api." + baseUrl;

        return baseUrl;
    }

    public static BasicConnectionArgs Create(string baseUrl, string database, string projectId, string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new Exception("Supplied username or password is empty.");

        return new BasicConnectionArgs(baseUrl, database, projectId, username, password);
    }

    public static BasicConnectionArgs CreateDefault(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetDefaultServer(), database, projectId, username, password);
    }

    public static BasicConnectionArgs CreateNoServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetNoServer(), database, projectId, username, password);
    }
    public static BasicConnectionArgs CreateEuServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetEuServer(), database, projectId, username, password);
    }
    public static BasicConnectionArgs CreateCaServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetCaServer(), database, projectId, username, password);
    }
    public static BasicConnectionArgs CreateUsServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetUsServer(), database, projectId, username, password);
    }
    public static BasicConnectionArgs CreateAuServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetAuServer(), database, projectId, username, password);
    }
}
