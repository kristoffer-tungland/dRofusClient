// ReSharper disable InconsistentNaming


using System.Globalization;

namespace dRofusClient;

public static class dRofusServers
{
    public static List<dRofusServer> GetServers()
    {
        return
        [
            dRofusServer.NoServer,
            dRofusServer.EuServer,
            dRofusServer.CaServer,
            dRofusServer.UsServer,
            dRofusServer.AuServer,
            dRofusServer.JpServer,
            dRofusServer.UkServer
        ];
    }

    public static string GetDefaultServer() => "db2.nosyko.no";
    public static string GetNoServer() => "https://api-no.drofus.com";
    public static string GetEuServer() => "https://api-eu.drofus.com";
    public static string GetCaServer() => "https://api-ca.drofus.com";
    public static string GetUsServer() => "https://api-us.drofus.com";
    public static string GetAuServer() => "https://api-au.drofus.com";
    public static string GetJpServer() => "https://api-jp.drofus.com";
    public static string GetUkServer() => "https://api-uk.drofus.com";

    public static string UriAdressToServer(string server)
    {
        server = server.TrimEnd('/');

        if (server == GetNoServer())
            server = GetDefaultServer();
        else
            server = server.Replace("http://", string.Empty).Replace("https://", string.Empty);

        return server;
    }
}

public record dRofusServer(string Name, string Adress, string Authority)
{
    public static dRofusServer NoServer => new("Nordics", dRofusServers.GetDefaultServer(), "https://ids-db2.drofus.com");
    public static dRofusServer EuServer => new("Europe", dRofusServers.GetEuServer(), "https://ids-eu.drofus.com");
    public static dRofusServer CaServer => new("Canada", dRofusServers.GetCaServer(), "https://ids-ca.drofus.com");
    public static dRofusServer UsServer => new("USA", dRofusServers.GetUsServer(), "https://ids-us.drofus.com");
    public static dRofusServer AuServer => new("Australia", dRofusServers.GetAuServer(), "https://ids-au.drofus.com");
    public static dRofusServer JpServer => new("Japan", dRofusServers.GetJpServer(), "https://ids-jp.drofus.com");
    public static dRofusServer UkServer => new("United Kingdom", dRofusServers.GetUkServer(), "https://ids-uk.drofus.com");

    internal static dRofusServer FromBaseUrl(string baseUrl)
    {
        if (baseUrl.Contains("api-no"))
            return NoServer;

        var servers = dRofusServers.GetServers();

        var apiAddress = baseUrl.Replace("api-", "ids-").TrimEnd('/');



        foreach (var server in servers)
        {
            if (apiAddress.Equals(server.Adress, StringComparison.OrdinalIgnoreCase))
            {
                return server;
            }
        }

        throw new ArgumentException($"No matching server found for base URL: {baseUrl}", nameof(baseUrl));
    }
}
