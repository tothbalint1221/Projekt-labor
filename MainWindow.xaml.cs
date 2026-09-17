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

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var users = await userRepository.GetAllAsync();
        Debug.WriteLine("Loaded users: " + users.Count);
        CustomerDataGrid.ItemsSource = users;
    }
}