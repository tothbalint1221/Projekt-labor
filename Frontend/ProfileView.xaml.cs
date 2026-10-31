using ServiceManagerApp.Repositories;
using ServiceManagerApp.Services.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceManagerApp.Frontend
{
   
    public partial class ProfileView : UserControl
    {
        private readonly UserSession _session;
        private readonly UserRepository _userRepository;
        private readonly IServiceProvider _services;

        public ProfileView(UserSession session, UserRepository userRepository, IServiceProvider services)
        {
            InitializeComponent();
            _session = session;
            _userRepository = userRepository;
            _services = services;

            btnSavePassword.IsEnabled = false;
            btnSignOut.IsEnabled = false;

            Loaded += ProfileView_Loaded;
            btnSignOut.Click += SignOut_Click;

        }

        private async void ProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            txtProfileName.Text = "Not Signed in";
            txtProfileRole.Text = string.Empty;
            txtProfileEmail.Text = string.Empty;
            txtProfilePhone.Text = string.Empty;
            txtProfileAddress.Text = string.Empty;

            txtCurrentPassword.IsEnabled = false;
            txtNewPassword.IsEnabled = false;
            txtConfirmPassword.IsEnabled = false;

            var currentUser = _session.CurrentUser;

            //közvetlen főablak inditásánal nincs adatb lekérdezés
            if (currentUser is null)
            {
                return;
            }
            btnSignOut.IsEnabled = true;
            try
            {
                var user = await _userRepository.GetAsync(currentUser.Id);

                if (user is null)
                {
                    txtProfileName.Text = "User not found";
                    return;
                }

                txtProfileName.Text = user.Name;
                txtProfileRole.Text = user.Role.ToString();
                txtProfileEmail.Text = user.Email;
                txtProfilePhone.Text = user.Phonenumber;
                txtProfileAddress.Text = user.Address;

                txtCurrentPassword.IsEnabled = true;
                txtNewPassword.IsEnabled = true;
                txtConfirmPassword.IsEnabled = true;


            }
            catch (Exception ex)
            {
                txtProfileName.Text = "Could not load profile";

                MessageBox.Show(
                    $"Could not load profile:\n\n{ex.Message}",
                    "Profile Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }
        }
        private void SignOut_Click(object sender,RoutedEventArgs e)
        {
            if(_session.CurrentUser is null)
            {
                return;
            }
            var mainWindow = Window.GetWindow(this);

            if (mainWindow is null)
            {
                return;
            }
            try 
            { 
                var signInWindow = _services.GetRequiredService<SignInWindow>();

                signInWindow.Show();
                Application.Current.MainWindow = signInWindow;

                _session.SignOut();

                txtCurrentPassword.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();

                mainWindow.Close();


            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"Could not sign out:\n\n{ex.Message}",
                    "Sign out error!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }



        }
    }
}
