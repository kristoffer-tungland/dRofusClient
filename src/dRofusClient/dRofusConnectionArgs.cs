// ReSharper disable InconsistentNaming
using System.Text;
using System.Text.RegularExpressions;

namespace dRofusClient;

public record dRofusConnectionArgs(string BaseUrl, string Database, string ProjectId, string AuthenticationHeader)
{
    public static dRofusConnectionArgs Create(string baseUrl, string database, string projectId, string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new Exception("Supplied username or password is empty.");

        var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

        return new dRofusConnectionArgs(baseUrl, database, projectId, "Basic " + base64String);
    }

    public static void ValidateServerAddress(string serverAddress)
    {
        var regex = new Regex(@"^https:\/\/api-[a-zA-Z]{2}\.drofus\.com\/?$");

        if (!regex.IsMatch(serverAddress))
            throw new InvalidOperationException("Invalid server address. Must be in the format https://api-no.drofus.com.");
    }

    public static dRofusConnectionArgs CreateNoServer(string database, string projectId, string username, string password)
    {
        return Create(GetNoServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateEuServer(string database, string projectId, string username, string password)
    {
        return Create(GetEuServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateCaServer(string database, string projectId, string username, string password)
    {
        return Create(GetCaServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateUsServer(string database, string projectId, string username, string password)
    {
        return Create(GetUsServer(), database, projectId, username, password);
    }
    public static dRofusConnectionArgs CreateAuServer(string database, string projectId, string username, string password)
    {
        return Create(GetAuServer(), database, projectId, username, password);
    }

    public static string GetDefaultServer() => GetNoServer();
    public static string GetNoServer() => "https://api-no.drofus.com";
    public static string GetEuServer() => "https://api-eu.drofus.com";
    public static string GetCaServer() => "https://api-ca.drofus.com";
    public static string GetUsServer() => "https://api-us.drofus.com";
    public static string GetAuServer() => "https://api-au.drofus.com/";
}