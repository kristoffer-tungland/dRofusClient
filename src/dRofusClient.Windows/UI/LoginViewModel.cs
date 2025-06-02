using dRofusClient.Exceptions;
using dRofusClient.Windows.Registries;
using Microsoft.Extensions.Logging;
using MVVMFluent;
using System.Collections.ObjectModel;

namespace dRofusClient.Windows.UI;

public class LoginViewModel : ViewModelBase
{
    private readonly IdRofusClient client;
    private readonly ILogger _logger;
    private Action<dRofusConnectionArgs>? _onLogin;
    private ModernLoginOptions? _modernLoginOptions;

    public LoginViewModel(IdRofusClientFactory clientFactory, ILogger logger)
    {
        _logger = logger;
        client = clientFactory.Create();
        var servers = dRofusServers.GetServers();

        foreach (var server in servers)
        {
            Servers.Add(server.Adress);
            ModernServers.Add(server);
        }
    }

    public string? Server { get => Get<string?>(); set => When(value).Changed(SetDatabases).Notify(Login).Set(); }
    public string? Database { get => Get<string?>(); set => When(value).Changed(SetPojects).Notify(Login).Set(); }
    public string? ProjectId { get => Get<string?>(); set => When(value).Notify(Login).Set(); }
    public string? Username { get => Get<string?>(); set => When(value).Notify(Login).Set(); }
    public string? Password { get => Get<string?>(); set => When(value).Notify(Login).Set(); }

    public bool UseModernSignIn { get => Get(false); set => When(value).Set(); }
    public bool RememberMe { get => Get(false); set => Set(value); }
    public string? ErrorMessage { get => Get<string?>(); set => Set(value); }

    public ObservableCollection<string> Servers { get; } = [];
    public ObservableCollection<string> Databases { get; } = [];
    public ObservableCollection<string> Projects { get; } = [];
    public ObservableCollection<dRofusServer> ModernServers { get; } = [];
    public dRofusServer? ModernServer { get => Get(dRofusServer.NoServer); set => Set(value); }

    public bool ServerIsEnabled { get => Get(true); set => Set(value); }
    public bool DatabaseIsEnabled { get => Get(true); set => Set(value); }
    public bool ProjectIsEnabled { get => Get(true); set => Set(value); }
    public bool ModernLoginAvailable{ get => Get(false); set => Set(value); }

    public void UseModernLogin(ModernLoginOptions options)
    {
        _modernLoginOptions = options;
        ModernLoginAvailable = true;
    }

    public void Initialize(Action<dRofusConnectionArgs> onLogin, string? server = default, string? database = default, string? projectId = default, string? username = default, string? password = default)
    {
        _onLogin = onLogin;

        if (string.IsNullOrWhiteSpace(server) == false)
        {
            server = dRofusServers.UriAdressToServer(server);
            ServerIsEnabled = false;
        }
        else
            ServerIsEnabled = true;

        Server = server ?? RegistryExtensions.GetActiveServer() ?? dRofusServers.GetDefaultServer();

        DatabaseIsEnabled = string.IsNullOrWhiteSpace(database) == true;

        Database = database ?? RegistryExtensions.GetActiveDatabase();

        ProjectIsEnabled = string.IsNullOrWhiteSpace(projectId) == true;

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
        _logger.LogError(exception, "Failed to login to dRofus");
    }

    private bool CanLogin()
    {
        if (UseModernSignIn)
        {
            return !string.IsNullOrWhiteSpace(ModernServer?.Adress) &&
                !string.IsNullOrWhiteSpace(Database) &&
                !string.IsNullOrWhiteSpace(ProjectId);

        }

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

        if (UseModernSignIn)
        {
            await ExecuteModernLogin(cancellationToken);
            return;
        }

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

            _onLogin?.Invoke(args);
        }
        catch (dRofusClientWrongCredentialsException)
        {
            ErrorMessage = "Failed to login to dRofus, please check your credentials.";
        }
    }

    private async Task ExecuteModernLogin(CancellationToken cancellationToken)
    {
        if (_modernLoginOptions == null)
            throw new InvalidOperationException("Modern login options are not set. Please call UseModernLogin before executing modern login.");

        var modernPromptHandler = new ModernPromptHandler(_modernLoginOptions, _logger);

        var result = await modernPromptHandler.HandleOidcAuthenticationAsync(ModernServer!, Database!, ProjectId!, cancellationToken);
        var args = ModernConnectionArgs.Create(ModernServer!, Database!, ProjectId!, result);
        _onLogin?.Invoke(args);
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
