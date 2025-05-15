using dRofusClient.WindowsCredentials.Registries;
using System;
using System.Security.Authentication;

namespace dRofusClient.WindowsCredentials;

public static class dRofusClientExtensions
{
    public static IdRofusClient Create(
        this dRofusClientFactory dRofusClientFactory, 
        string server, 
        string database, 
        string? projectId = default)
    {
        projectId ??= "01";
        var args = dRofusClientFactory.GetConnectionArgs(server, database, projectId);
        return dRofusClientFactory.Create(args);
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
        SaveCredentials(serverAddress, username, password);
    }

    public static void SaveCredentials(string server, string username, string password)
    {
        BasicCredentialsExtensions.SaveCredential(server, username, password);
    }
}