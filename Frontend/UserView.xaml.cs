using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ServiceManagerApp.Repositories;

namespace ServiceManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class UserView : UserControl
{
    private readonly UserRepository userRepository;
    private ObservableCollection<User> users = new ObservableCollection<User>();
    public UserView(UserRepository userRepository)
    {
        InitializeComponent();
        this.userRepository = userRepository;

        cmbUserRole.ItemsSource = Enum.GetValues<Role>();
        cmbUserRole.SelectedItem = Role.Technician;

        Loaded += async (s, e) => await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            var _users = await userRepository.GetAllAsync();

            users.Clear();

            if (_users != null)
            {
                foreach (var user in _users)
                {
                    users.Add(user);
                }
            }

            dataGridUsers.ItemsSource = users;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load users:\n\n{ex.Message}",
                "User Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnAddUserAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text) ||
                string.IsNullOrWhiteSpace(txtUserPassword.Text) ||
                string.IsNullOrWhiteSpace(txtUserPhone.Text) ||
                string.IsNullOrWhiteSpace(txtUserEmail.Text) ||
                string.IsNullOrWhiteSpace(txtUserAddress.Text))
            {
                MessageBox.Show("Please fill out every field.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbUserRole.SelectedItem is not Role roleValue)
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new User
            {
                Name = txtUserName.Text,
                Password = txtUserPassword.Text,
                Phonenumber = txtUserPhone.Text,
                Email = txtUserEmail.Text,
                Address = txtUserAddress.Text,
                Role = roleValue
            };

            await userRepository.CreateAsync(newUser);

            txtUserName.Clear();
            txtUserPassword.Clear();
            txtUserPhone.Clear();
            txtUserEmail.Clear();
            txtUserAddress.Clear();

            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not add user:\n\n{ex.Message}",
                "User Add Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnDeleteUserAsync(object sender, RoutedEventArgs e)
    {
        MessageBoxResult userResponse = MessageBox.Show("Are you sure you want to delete the user?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (userResponse != MessageBoxResult.Yes) return;
        try
        {
            if (sender is Button button && button.DataContext is User selectedUser)
            {
                await userRepository.DeleteAsync(selectedUser);

                await LoadUsersAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}