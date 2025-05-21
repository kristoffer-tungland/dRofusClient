using System.Text;

namespace dRofusClient;

public record dRofusConnectionArgs(string BaseUrl, string Database, string ProjectId, string? AuthenticationHeader = default)
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

    public static dRofusConnectionArgs Create(string baseUrl, string database, string projectId, string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new Exception("Supplied username or password is empty.");

        var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

        return new dRofusConnectionArgs(baseUrl, database, projectId, "Basic " + base64String);
    }

    public static dRofusConnectionArgs CreateDefault(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetDefaultServer(), database, projectId, username, password);
    }

    public static dRofusConnectionArgs CreateNoServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetNoServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateEuServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetEuServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateCaServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetCaServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateUsServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetUsServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateAuServer(string database, string projectId, string username, string password)
    {
        return Create(dRofusServers.GetAuServer(), database, projectId, username, password);
    }
}
