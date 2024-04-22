using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

namespace dRofusClient.WindowsCredentials;

public static class dRofusRegistryExtensions
{
    /// <summary>
    /// Get stored databases from the registry
    /// Computer\HKEY_CURRENT_USER\SOFTWARE\dRoFUs\Projects\{server}
    /// </summary>
    /// <returns>List of databases</returns>
    public static List<string> GetStoredDatabases(string server)
    {
        dRofusConnectionArgs.ValidateServerAddress(server);
        server = server.Replace("http://", "").Replace("https://", "");
        var key = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\dRoFUs\Projects\{server}");
        return key is null ? [] : key.GetSubKeyNames().ToList();
    }

    /// <summary>
    /// Get stored projects from the registry under the server and database
    /// Computer\HKEY_CURRENT_USER\SOFTWARE\dRoFUs\Projects\{server}\{database}
    /// </summary>
    /// <returns>List of projects ids</returns>
    public static List<string> GetStoredProjects(string server, string database)
    {
        dRofusConnectionArgs.ValidateServerAddress(server);
        if (string.IsNullOrWhiteSpace(database))
            throw new Exception("No database provided.");
        server = server.Replace("http://", "").Replace("https://", "");
        var key = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\dRoFUs\Projects\{server}\{database}");
        return key is null ? [] : key.GetSubKeyNames().ToList();
    }
}