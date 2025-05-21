using Microsoft.Win32;

namespace dRofusClient.Windows.Registries;

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
        server = dRofusServers.UriAdressToServer(server);
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

        server = dRofusServers.UriAdressToServer(server);
        var key = Registry.CurrentUser.OpenSubKey(GetDatabaseRegistryPath(server, database));
        return key is null ? [] : [.. key.GetSubKeyNames()];
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

    public static string? GetActiveDatabase()
        {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\ODBC\ODBC.INI\rofus");
        return key?.GetValue("Database") as string;
    }

    public static string? GetActiveServer()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\ODBC\ODBC.INI\rofus");
        return key?.GetValue("Servername") as string;
    }

    public static string? GetActiveProjectId()
    {
        var database = GetActiveDatabase();

        if (string.IsNullOrWhiteSpace(database))
            return null;

        var server = GetActiveServer();

        if (string.IsNullOrWhiteSpace(server))
            return null;

        var storedProjects = GetStoredProjects(server!, database!);

        if (storedProjects.Count == 0)
            return null;

        return storedProjects[0];
    }

    public static void StoreUsername(string username)
    {
        using var key = Registry.CurrentUser.CreateSubKey(@"Software\ODBC\ODBC.INI\rofus");
        key?.SetValue("Username", username);
    }

    internal static void StoreServer(string server)
    {
        using var key = Registry.CurrentUser.CreateSubKey(@"Software\ODBC\ODBC.INI\rofus");
        key?.SetValue("Servername", server);
    }

    internal static void StoreDatabase(string database)
    {
        using var key = Registry.CurrentUser.CreateSubKey(@"Software\ODBC\ODBC.INI\rofus");
        key?.SetValue("Database", database);
    }
}
