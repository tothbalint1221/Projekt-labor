using System.Diagnostics;
using System.Windows;
using ServiceManagerApp.Repositories;
using Microsoft.Extensions.DependencyInjection;
using ServiceManagerApp.Frontend;
using ServiceManagerApp.Services.Users;

namespace ServiceManagerApp;

/// <summary>
/// Interaction logic for SignInWindow.xaml
/// </summary>
public partial class SignInWindow : Window
{
    private readonly UserRepository _userRepository;
    private readonly IServiceProvider _services;
    private readonly UserSession _session;

    public SignInWindow(UserRepository userRepository, IServiceProvider service, UserSession session)
    {
        InitializeComponent();
        this._userRepository = userRepository;
        this._services = service;
        this._session = session;
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(EmailTextBox.Text))
        {
            MessageBox.Show("Please enter your username");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordInput.Password))
        {
            MessageBox.Show("Please enter your password");
            return; 
        }

        LoginButton.IsEnabled = false; //letiltja a login gombot :D

        try
        {
            var result = await _userRepository.LoginAsync(
                EmailTextBox.Text,
                PasswordInput.Password);

            if(result.User == null)
            {
                MessageBox.Show(result.Reason);
                return;
            }

            _session.SignIn(result.User);
            var primaryWindow = _services.GetRequiredService<PrimaryWindow>();
            Application.Current.MainWindow = primaryWindow;
            primaryWindow.Show();
            Close();

        }
        catch (Exception ex)
        {
            _session.SignOut();

            MessageBox.Show($"Login failed:({ex.Message}), Try again later!");
        }
        finally { LoginButton.IsEnabled = true; }
    }
}