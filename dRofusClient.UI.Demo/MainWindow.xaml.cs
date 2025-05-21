#if DEBUG
using Microsoft.Extensions.Configuration;
#endif
using System.Windows;

namespace dRofusClient.UI.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
#if DEBUG
            var config = new ConfigurationBuilder()
                .AddUserSecrets("9a317e62-4dc9-48e2-841d-a3eaa2ebdb88")
                .Build();

            Environment.SetEnvironmentVariable("OAUTH2_AUTHORITY", config["OAuth2:Authority"]);
            Environment.SetEnvironmentVariable("OAUTH2_CLIENTID", config["OAuth2:ClientId"]);
            Environment.SetEnvironmentVariable("OAUTH2_CLIENTSECRET", config["OAuth2:ClientSecret"]);
            Environment.SetEnvironmentVariable("OAUTH2_SCOPE", config["OAuth2:Scope"]);
#endif


            DataContext = new MainViewModel(new dRofusClientFactory(), new Microsoft.Extensions.Logging.Abstractions.NullLogger<MainViewModel>());
            InitializeComponent();
        }
    }
}