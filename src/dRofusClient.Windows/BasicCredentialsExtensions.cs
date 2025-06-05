using Meziantou.Framework.Win32;

namespace dRofusClient.Windows;

public static class BasicCredentialsExtensions
{
    public static Credential ReadCredential(string server, string username)
    {
        return CredentialManager.ReadCredential(CreateTarget(server, username));
    }

    public static void SaveCredential(string server, string username, string password)
    {
        CredentialManager.WriteCredential(CreateTarget(server, username), username, password, "dRofus login credentials", CredentialPersistence.LocalMachine);
    }

    public static void DeleteCredentials(string server, string username)
    {
        try
        {
            CredentialManager.DeleteCredential(CreateTarget(server, username));
        }
        catch
        { 
            //Ignore}
        }
    }

    private static string CreateTarget(string server, string username)
    {
        server = server.TrimEnd('/');

        if (server == "https://api-no.drofus.com")
            server = "db2.nosyko.no";
        else
            server = server.Replace("http://", string.Empty).Replace("https://", string.Empty);

        return "drofus://" + username + "@" + server;
    }
}