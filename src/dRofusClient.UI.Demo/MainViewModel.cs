using dRofusClient.Windows.UI;
using Microsoft.Extensions.Logging;
using MVVMFluent;
using System.Windows;
using dRofusClient.Projects;
using dRofusClient.Windows;

namespace dRofusClient.UI.Demo;

internal class MainViewModel(IdRofusClientFactory clientFactory,  ILogger logger) : ViewModelBase
{
    private LoginWindow? _loginWindow;

    public string? ProjectName { get => Get("<Project Name>"); set => Set(value); }

    private IdRofusClient client = clientFactory.Create(new DialogPromptHandler(clientFactory, logger));

    public IFluentCommand ShowLoginCommand => Do(ShowLogin);

    public IFluentCommand ClearAuthenticationCommand => Do(() =>
    {
        client.ClearAuthentication();
        MessageBox.Show("Cleared authentication!");
    });

    public IFluentCommand UseModernSignInCommand => Do(() =>
    {
        client = clientFactory.Create(new ModernPromptHandler());
    });

    public IFluentCommand LogoutCommand => Do(() =>
    {
        client.Logout();
        client = clientFactory.Create(new DialogPromptHandler(clientFactory, logger));
        MessageBox.Show("Logged out!");
    });

    public IAsyncFluentCommand GetProjectNameCommand => Do(async (cancellationToken) =>
    {
        ProjectName = "Loading...";
        var project = await client.GetProjectAsync(cancellationToken: cancellationToken);
        ProjectName = project?.Name ?? "No project found";
    }).Handle(ex => MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));

    private void ShowLogin()
    {
        if (_loginWindow != null)
            return;

        var viewModel = new LoginViewModel(clientFactory,logger);

        viewModel.Initialize(OnLogin);

        _loginWindow = new LoginWindow(viewModel)
        {
            Owner = Application.Current.MainWindow,
        };

        _loginWindow.Closed += (s, e) =>
        {
            _loginWindow = null;
        };

        _loginWindow.ShowDialog();
    }

    private void OnLogin(dRofusConnectionArgs args)
    {
        client = clientFactory.Create(args);
        MessageBox.Show("Login successful!");
        _loginWindow?.Close();
    }

}
