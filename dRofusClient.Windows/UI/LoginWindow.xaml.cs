using System.Windows;

namespace dRofusClient.Windows.UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            var view = new LoginView()
            {
                DataContext = viewModel
            };
            Content = view;
            InitializeComponent();
        }
    }
}
