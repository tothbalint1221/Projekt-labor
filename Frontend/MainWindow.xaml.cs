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

    }
}