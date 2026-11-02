using System.Windows;
using ServiceManagerApp.Repositories;
using ServiceManagerApp.Services.Users;

namespace ServiceManagerApp.Frontend;

public partial class UserEdit : Window
{
    private readonly User user;
    private readonly UserRepository userRepository;
    private readonly UserSession userSession;
    private bool requireCurrentPassword;

    public UserEdit(User user, UserRepository userRepository, UserSession userSession)
    {
        InitializeComponent();
        this.user = user;
        this.userRepository = userRepository;
        this.userSession = userSession;

        txtUserId.Text = user.Id.ToString();
        txtUserName.Text = user.Name;
        txtUserPhone.Text = user.Phonenumber;
        txtUserEmail.Text = user.Email;
        txtUserAddress.Text = user.Address;
        cmbUserRole.ItemsSource = Enum.GetValues<Role>();
        cmbUserRole.SelectedItem = user.Role;

        if (userSession.CurrentUser?.Id != user.Id)
        {
            lblCurrentPassword.Visibility = Visibility.Collapsed;
            txtCurrentPassword.Visibility = Visibility.Collapsed;
            requireCurrentPassword = false;
        }
        else
        {
            lblCurrentPassword.Visibility = Visibility.Visible;
            txtCurrentPassword.Visibility = Visibility.Visible;
            requireCurrentPassword = true;
        }
    }

    private void SetUserDetails(string name, string phone, string email, string address, Role role)
    {
        user.Name = name;
        user.Phonenumber = phone;
        user.Email = email;
        user.Address = address;
        user.Role = role;
    }

    private void SetSaveButtonsEnabled(bool isEnabled)
    {
        btnSaveUser.IsEnabled = isEnabled;
        btnSavePassword.IsEnabled = isEnabled;
    }

    private async void BtnSaveUserAsync(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUserName.Text) ||
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

        var previous = (user.Name, user.Phonenumber, user.Email, user.Address, user.Role);

        SetSaveButtonsEnabled(false);

        try
        {
            SetUserDetails(
                txtUserName.Text,
                txtUserPhone.Text,
                txtUserEmail.Text.Trim(),
                txtUserAddress.Text,
                roleValue);

            await userRepository.UpdateAsync(user);

            MessageBox.Show("The user has been updated.", "User Update", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            SetUserDetails(previous.Name, previous.Phonenumber, previous.Email, previous.Address, previous.Role);

            MessageBox.Show(
                $"Could not update user:\n\n{ex.Message}",
                "User Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            SetSaveButtonsEnabled(true);
        }
    }

    private async void BtnSavePasswordAsync(object sender, RoutedEventArgs e)
    {
        if ((requireCurrentPassword && string.IsNullOrWhiteSpace(txtCurrentPassword.Password)) ||
            string.IsNullOrWhiteSpace(txtNewPassword.Password) ||
            string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
        {
            MessageBox.Show("Please fill in all password fields", "Missing password", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (txtNewPassword.Password != txtConfirmPassword.Password)
        {
            MessageBox.Show("The new passwords do not match", "Password mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SetSaveButtonsEnabled(false);

        try
        {
            if (requireCurrentPassword)
            {
                var result = await userRepository.ChangePasswordAsync(
                    user.Id,
                    txtCurrentPassword.Password,
                    txtNewPassword.Password);

                if (!result.Success)
                {
                    MessageBox.Show(result.Error, "Password change", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                await userRepository.ForceChangePasswordAsync(user.Id, txtNewPassword.Password);
            }

            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();

            MessageBox.Show("The password has been changed.", "Password change", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not change password:\n\n{ex.Message}",
                "Password change error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            SetSaveButtonsEnabled(true);
        }
    }
}
