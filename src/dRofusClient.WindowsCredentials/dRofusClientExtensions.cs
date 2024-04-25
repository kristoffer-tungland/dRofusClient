using Meziantou.Framework.Win32;
using System;

// ReSharper disable InconsistentNaming

namespace dRofusClient.WindowsCredentials;

public static class dRofusClientExtensions
{
    public static IdRofusClient Create(
        this dRofusClientFactory dRofusClientFactory, 
        string serverAddress, 
        string database, 
        string projectId)
    {
        var args = dRofusClientFactory.GetConnectionArgs(serverAddress, database, projectId);
        return dRofusClientFactory.Create(args);
    }

    public static dRofusConnectionArgs GetConnectionArgs(this dRofusClientFactory dRofusClientFactory, string serverAddress, string database, string projectId)
    {
        if (string.IsNullOrWhiteSpace(serverAddress))
            throw new InvalidOperationException("No server address provided.");

        dRofusConnectionArgs.ValidateServerAddress(serverAddress);

        var credential = CredentialManager.ReadCredential(serverAddress);

        if (credential is null)
            throw new InvalidOperationException("No credentials found for dRofus database.");

        var username = credential.UserName;
        var password = credential.Password;

        return dRofusConnectionArgs.Create(serverAddress, database, projectId, username, password);
    }

    public static void SaveCredentials(this IdRofusClient client, string username, string password)
    {
        var serverAddress = client.GetBaseUrl();
        SaveCredentials(serverAddress, username, password);
    }

    public static void SaveCredentials(string serverAddress, string username, string password)
    {
        dRofusConnectionArgs.ValidateServerAddress(serverAddress);
        const string comment = "dRofus login credentials";
        CredentialManager.WriteCredential(serverAddress, username, password, comment, CredentialPersistence.LocalMachine);
    }
}