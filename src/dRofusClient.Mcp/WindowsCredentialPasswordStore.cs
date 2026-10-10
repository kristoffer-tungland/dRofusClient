using Meziantou.Framework.Win32;

namespace dRofusClient.Mcp;

public interface IWindowsCredentialPasswordStore
{
    string? ReadPassword(string server, string username);
}

public sealed class WindowsCredentialPasswordStore : IWindowsCredentialPasswordStore
{
    public string? ReadPassword(string server, string username)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Windows Credential Manager requires Windows.");
        return CredentialManager.ReadCredential(CreateTarget(server, username))?.Password;
    }

    public static string CreateTarget(string server, string username)
    {
        // Match dRofusClient.Windows.BasicCredentialsExtensions without taking a WPF dependency.
        return "drofus://" + username + "@" + dRofusServers.UriAdressToServer(server);
    }
}
