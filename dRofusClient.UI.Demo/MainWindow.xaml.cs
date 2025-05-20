using dRofusClient.Windows.UI;
using Microsoft.Extensions.Logging.Abstractions;
using System.Windows;

namespace dRofusClient.UI.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginWindow? _loginWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_loginWindow != null)
                return;

            var factory = new dRofusClientFactory();
            var client = factory.Create();
            var viewModel = new LoginViewModel(client, NullLogger.Instance);

            viewModel.Initialize(OnLogin);

            _loginWindow = new LoginWindow(viewModel)
            {
                Owner = this
            };

            _loginWindow.Closed += (s, e) =>
            {
                _loginWindow = null;
            };

            _loginWindow.ShowDialog();
        }

        private void OnLogin(bool obj)
        {
            if (obj)
            {
                MessageBox.Show("Login successful!");
                _loginWindow?.Close();
            }
            else
            {
                MessageBox.Show("Login failed!");
            }
        }
    }
}