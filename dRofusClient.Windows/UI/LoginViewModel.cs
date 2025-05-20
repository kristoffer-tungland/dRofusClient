using dRofusClient.Exceptions;
using dRofusClient.Windows.Registries;
using Microsoft.Extensions.Logging;
using MVVMFluent;
using System.Collections.ObjectModel;

namespace dRofusClient.Windows.UI;

public class LoginViewModel(IdRofusClient client, ILogger logger) : ViewModelBase
{
    private Action<bool>? _onLogin;

    public string? Server { get => Get<string?>(); set => When(value).Changed(SetDatabases).Notify(Login).Set(); }
    public string? Database { get => Get<string?>(); set => When(value).Changed(SetPojects).Notify(Login).Set(); }
    public string? ProjectId { get => Get<string?>(); set => When(value).Notify(Login).Set(); }
    public string? Username { get => Get<string?>(); set => When(value).Notify(Login).Set(); }
    public string? Password { get => Get<string?>(); set => When(value).Notify(Login).Set(); }

    public bool UseModernSignIn { get => Get(false); set => When(value).Set(); }
    public bool RememberMe { get => Get(false); set => Set(value); }
    public string? ErrorMessage { get => Get<string?>(); set => Set(value); }

    public ObservableCollection<string> Servers { get; } = new(dRofusServers.GetServers());
    public ObservableCollection<string> Databases { get; } = [];
    public ObservableCollection<string> Projects { get; } = [];
    public ObservableCollection<string> ModernServers { get; } = new() { "Nordics", "Americas", "Australia" };
    public string? ModernServer { get => Get<string?>(); set => When(value).Set(); }

    public void Initialize(Action<bool> onLogin, string? server = null, string? database = null, string? projectId = null, string? username = null, string? password = null)
    {
        _onLogin = onLogin;
        Server = server ?? RegistryExtensions.GetActiveServer() ?? dRofusServers.GetDefaultServer();
        Database = database ?? RegistryExtensions.GetActiveDatabase();
        ProjectId = projectId ?? RegistryExtensions.GetActiveProjectId();
        Username = username ?? RegistryExtensions.GetUsername();

        Password = password;

        var credential = string.IsNullOrEmpty(Username) ?
            null :
            BasicCredentialsExtensions.ReadCredential(Server, Username);

        RememberMe = credential != null;

        if (string.IsNullOrEmpty(Password) && credential != null)
            Password = credential.Password;
    }

    public IFluentCommand Login => Do(ExecuteLogin)
        .If(CanLogin).Handle(LoginFailed);

    private void LoginFailed(Exception exception)
    {
        ErrorMessage = "An unexpected error occurred!";
        logger.LogError(exception, "Failed to login to dRofus");
    }

    private bool CanLogin()
    {
        return !string.IsNullOrWhiteSpace(Server) &&
            !string.IsNullOrWhiteSpace(Database) &&
            !string.IsNullOrWhiteSpace(ProjectId) &&
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Password);
    }

    private async Task ExecuteLogin(CancellationToken cancellationToken)
    {
        if (!CanLogin())
            return;

        var args = dRofusConnectionArgs.Create(Server!, Database!, ProjectId!, Username!, Password!);

        try
        {
            await client.Login(args, cancellationToken);

            if (RememberMe)
            {
                RegistryExtensions.StoreUsername(Username!);
                RegistryExtensions.StoreServer(Server!);
                RegistryExtensions.StoreDatabase(Database!);

                client.SaveCredentials(Username!, Password!);
            }
            else
            {
                client.SaveCredentials(Username!, string.Empty);
            }

            _onLogin?.Invoke(true);
        }
        catch (dRofusClientLoginException ex)
        {
            ErrorMessage = "Failed to login to dRofus, please check your credentials.";
            logger.LogError(ex, "Failed to login to dRofus: {Message}", ex.Message);
            _onLogin?.Invoke(false);
        }
    }

    private void SetDatabases(string? obj)
    {
        if (string.IsNullOrWhiteSpace(Server))
            return;

        Databases.Clear();

        var databases = RegistryExtensions.GetStoredDatabases(Server!);

        foreach (var database in databases)
            Databases.Add(database);
    }

    private void SetPojects(string? obj)
    {
        if (string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(Database))
            return;
        var projects = RegistryExtensions.GetStoredProjects(Server!, Database!);

        Projects.Clear();

        if (projects.Count == 0)
            return;
        
        foreach (var project in projects)
            Projects.Add(project);

        ProjectId = projects[0];
    }
}
