// ReSharper disable InconsistentNaming

namespace dRofusClient;

public static class dRofusServers
{
    public static List<string> GetServers()
    {
        return
        [
            GetDefaultServer(),
            GetNoServer(),
            GetEuServer(),
            GetCaServer(),
            GetUsServer(),
            GetAuServer()
        ];
    }

    public static string GetDefaultServer() => "db2.nosyko.no";
    public static string GetNoServer() => "https://api-no.drofus.com";
    public static string GetEuServer() => "https://api-eu.drofus.com";
    public static string GetCaServer() => "https://api-ca.drofus.com";
    public static string GetUsServer() => "https://api-us.drofus.com";
    public static string GetAuServer() => "https://api-au.drofus.com/";

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
