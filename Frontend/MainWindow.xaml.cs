using System.Diagnostics;
using System.Windows;
using ServiceManagerApp.Repositories;

namespace ServiceManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly UserRepository userRepository;

    public MainWindow(UserRepository userRepository)
    {
        InitializeComponent();
        this.userRepository = userRepository;
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(UsernameTextBox.Text))
        {
            MessageBox.Show("Please enter your username");
            return;
        }


        if (string.IsNullOrWhiteSpace(PasswordInput.Password))
        {
            MessageBox.Show("Please enter your password");
            //MessageBox.Show("Please enter a password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return; 
        }

        var menu = new Frontend.MenuWindow();
        menu.Show(); //ez nyitja meg a menut
        Close(); //Ez bezárja a logint, fontos!

    }
}