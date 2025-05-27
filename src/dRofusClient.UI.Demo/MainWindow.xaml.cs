using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace dRofusClient.UI.Demo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets("9a317e62-4dc9-48e2-841d-a3eaa2ebdb88")
            .Build();

        var clientId = config["OAuth2:ClientId"];
        var clientSecret = config["OAuth2:ClientSecret"];
        var scope = config["OAuth2:Scope"];
        var redirectUri = config["OAuth2:RedirectUri"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) ||
            string.IsNullOrEmpty(scope) || string.IsNullOrEmpty(redirectUri))
        {
            throw new InvalidOperationException("OAuth2 configuration is missing or incomplete.");
        }

        var modernLoginOptions = new ModernLoginOptions(clientId, clientSecret, scope, redirectUri);

        DataContext = new MainViewModel(modernLoginOptions, new dRofusClientFactory(), new NullLogger<MainViewModel>());
        InitializeComponent();
    }
}