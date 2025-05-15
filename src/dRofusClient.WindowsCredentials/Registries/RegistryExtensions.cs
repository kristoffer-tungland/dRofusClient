using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace dRofusClient.WindowsCredentials.Registries;

public static class RegistryExtensions
{
    public const string dRofusRegistryPath = @"SOFTWARE\dRoFUs";
    public const string dRofusRegistryPathProjects = @$"{dRofusRegistryPath}\Projects";

    public static string GetServerRegistryPath(string server) => $@"{dRofusRegistryPathProjects}\{server}";
    public static string GetDatabaseRegistryPath(string server, string database) => $@"{dRofusRegistryPathProjects}\{server}\{database}";
    public static string GetProjectRegistryPath(string server, string database, string projectId) => $@"{dRofusRegistryPathProjects}\{server}\{database}\{projectId}";

    /// <summary>
    /// Get stored databases from the registry
    /// Computer\HKEY_CURRENT_USER\SOFTWARE\dRoFUs\Projects\{server}
    /// </summary>
    /// <returns>List of databases</returns>
    public static List<string> GetStoredDatabases(string server)
    {
        server = NormalizeServerAddress(server);
        var key = Registry.CurrentUser.OpenSubKey(GetServerRegistryPath(server));
        return key is null ? [] : [.. key.GetSubKeyNames()];
    }

    /// <summary>
    /// Get stored projects from the registry under the server and database
    /// Computer\HKEY_CURRENT_USER\SOFTWARE\dRoFUs\Projects\{server}\{database}
    /// </summary>
    /// <returns>List of projects ids</returns>
    public static List<string> GetStoredProjects(string server, string database)
    {
        if (string.IsNullOrWhiteSpace(database))
            throw new Exception("No database provided.");

        server = NormalizeServerAddress(server);
        var key = Registry.CurrentUser.OpenSubKey(GetDatabaseRegistryPath(server, database));
        return key is null ? [] : [.. key.GetSubKeyNames()];
    }

    private static string NormalizeServerAddress(string server)
    {
        if (server == "https://api-no.drofus.com")
            server = "db2.nosyko.no";
        else
            server = server.Replace("http://", string.Empty).Replace("https://", string.Empty);

        return server;
    }

    /// <summary>
    /// Get stored username from the registry
    /// Computer\HKEY_CURRENT_USER\SOFTWARE\ODBC\ODBC.INI\rofus
    /// </summary>
    /// <returns>Username</returns>
    public static string? GetUsername()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\ODBC\ODBC.INI\rofus");
        return key?.GetValue("Username") as string;
    }
}
