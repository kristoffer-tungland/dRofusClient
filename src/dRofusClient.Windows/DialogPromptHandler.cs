using dRofusClient.Exceptions;
using dRofusClient.Windows.UI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Windows;

namespace dRofusClient.Windows;

public class DialogPromptHandler(IdRofusClientFactory clientFactory, ModernLoginOptions? modernLoginOptions = default, ILogger? logger = default) : ILoginPromptHandler
{
    private LoginWindow? _loginWindow;

    public async Task Handle(IdRofusClient client, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<dRofusConnectionArgs?>();

        void OnLogin(dRofusConnectionArgs loginArgs)
        {
            tcs.TrySetResult(loginArgs);
            _loginWindow?.Close();
        }

        var viewModel = new LoginViewModel(clientFactory, logger ?? NullLogger.Instance);

        string? database;
        string? projectId;

        try
        {
            database = client.GetDatabaseAndProjectId().database;
            projectId = client.GetDatabaseAndProjectId().projectId;
        }
        catch (Exception)
        {
            database = null;
            projectId = null;
        }

        if(modernLoginOptions != null)
            viewModel.UseModernLogin(modernLoginOptions);

        viewModel.Initialize(OnLogin, server: client.GetBaseUrl(), database: database, projectId: projectId);

        _loginWindow = new LoginWindow(viewModel);

        if (Application.Current?.MainWindow != null)
        {
            _loginWindow.Owner = Application.Current.MainWindow;
        }

        _loginWindow.Closed += (s, e) =>
        {
            _loginWindow = null;
            // If login was not successful, set result to null if not already set
            tcs.TrySetResult(null);
        };

        _loginWindow.Show();

        var result = await tcs.Task.ConfigureAwait(false);

        if (result == null)
        {
            throw new dRofusClientLoginException("Login was cancelled or failed.");
        }

        client.Setup(result);
    }
}