using System.Windows;
using System.Windows.Controls;

namespace dRofusClient.Windows.UI;

/// <summary>
/// Interaction logic for LoginView.xaml
/// </summary>
public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        // Set PasswordBox initial value from ViewModel.Password if available
        this.Loaded += (s, e) =>
        {
            if (DataContext is LoginViewModel vm && !string.IsNullOrEmpty(vm.Password))
                PasswordBox.Password = vm.Password;
        };
    }

    private void ForgotPassword_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        // open https://www.drofus.com/en/reset-password

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = e.Uri.ToString(),
            UseShellExecute = true
        });
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            // Prevent unnecessary updates
            if (PasswordBox.Password != vm.Password)
                vm.Password = PasswordBox.Password;
        }
    }
}
