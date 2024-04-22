using Meziantou.Framework.Win32;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace dRofusClient.WindowsCredentials;

public static class dRofusClientExtensions
{
    public static void ValidateServerAddress(string serverAddress)
    {
        var regex = new Regex(@"^https:\/\/api-[a-zA-Z]{2}\.drofus\.com\/?$");

        if (!regex.IsMatch(serverAddress))
            throw new InvalidOperationException("Invalid server address. Must be in the format https://api-no.drofus.com.");
    }

    public static async Task<IdRofusClient> Create(
        this dRofusClientFactory dRofusClientFactory, 
        string serverAddress, 
        string database, 
        string projectId, 
        CancellationToken cancellationToken = default)
    {
        var args = dRofusClientFactory.GetConnectionArgs(serverAddress, database, projectId);
        return await dRofusClientFactory.Create(args, cancellationToken);
    }

    public static dRofusConnectionArgs GetConnectionArgs(this dRofusClientFactory dRofusClientFactory, string serverAddress, string database, string projectId)
    {
        if (string.IsNullOrWhiteSpace(serverAddress))
            throw new InvalidOperationException("No server address provided.");

        ValidateServerAddress(serverAddress);

        var credential = CredentialManager.ReadCredential(serverAddress);

        if (credential is null)
            throw new InvalidOperationException("No credentials found for dRofus database.");

        var username = credential.UserName;
        var password = credential.Password;

        return dRofusConnectionArgs.Create(serverAddress, database, projectId, username, password);
    }

    public static void SaveConnectionArgs(this IdRofusClient client, string username, string password)
    {
        var serverAddress = client.GetBaseUrl();
        SaveCredentials(serverAddress, username, password);
    }

    public static void SaveCredentials(string serverAddress, string username, string password)
    {
        ValidateServerAddress(serverAddress);
        const string comment = "dRofus login credentials";
        CredentialManager.WriteCredential(serverAddress, username, password, comment, CredentialPersistence.LocalMachine);
    }
}