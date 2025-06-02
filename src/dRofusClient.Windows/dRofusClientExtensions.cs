using dRofusClient.Windows.Registries;
using System.Security.Authentication;

namespace dRofusClient.Windows;

public static class dRofusClientExtensions
{
    public static IdRofusClient Create(
        this dRofusClientFactory dRofusClientFactory, 
        string server, 
        string database, 
        string? projectId = default,
        ILoginPromptHandler? loginPromtHandler = default)
    {
        projectId ??= RegistryExtensions.GetStoredProjects(server, database).FirstOrDefault() ?? "01";
        var args = dRofusClientFactory.GetConnectionArgs(server, database, projectId);
        return dRofusClientFactory.Create(args, loginPromtHandler);
    }

    public static IdRofusClient CreateActive(
        this dRofusClientFactory dRofusClientFactory)
    {
        var server = RegistryExtensions.GetActiveServer();

        if (string.IsNullOrWhiteSpace(server))
            throw new InvalidOperationException("No server address is active.");

        var database = RegistryExtensions.GetActiveDatabase();

        if (string.IsNullOrWhiteSpace(database))
            throw new InvalidOperationException("No database is active.");

        var projectId = RegistryExtensions.GetActiveProjectId();

        return dRofusClientFactory.Create(server!, database!, projectId);
    }

    public static dRofusConnectionArgs GetConnectionArgs(this dRofusClientFactory dRofusClientFactory, string server, string database, string projectId)
    {
        if (string.IsNullOrWhiteSpace(server))
            throw new InvalidOperationException("No server address provided.");

        var username = RegistryExtensions.GetUsername();

        if (username is null || string.IsNullOrWhiteSpace(username))
            throw new InvalidCredentialException("You must log in to dRofus first.");

        var credential = BasicCredentialsExtensions.ReadCredential(server, username) 
            ?? throw new InvalidOperationException("No credentials found for dRofus database.");

        var password = credential.Password;

        return dRofusConnectionArgs.Create(server, database, projectId, username, password);
    }

    public static void SaveCredentials(this IdRofusClient client, string username, string password)
    {
        var serverAddress = client.GetBaseUrl();

        if (string.IsNullOrWhiteSpace(serverAddress))
            throw new InvalidOperationException("dRofus client has no server address.");

        BasicCredentialsExtensions.SaveCredential(serverAddress!, username, password);
    }

    public static string? ReadPassword(this IdRofusClient client, string username)
    {
        var serverAddress = client.GetBaseUrl();

        if (string.IsNullOrWhiteSpace(serverAddress))
            return null;

        var credential = BasicCredentialsExtensions.ReadCredential(serverAddress!, username);
        return credential?.Password;
    }
}
