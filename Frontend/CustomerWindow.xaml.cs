using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ServiceManagerApp.Repositories;

namespace ServiceManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class CustomerWindow : Window
{
    private readonly CustomerRepository customerRepository;
    private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
    public CustomerWindow(CustomerRepository customerRepository)
    {
        InitializeComponent();
        this.customerRepository = customerRepository;

        Loaded += async (s, e) => await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            var _customers = await customerRepository.GetAllAsync();

            customers.Clear();

            if (_customers != null)
            {
                foreach (var customer in _customers)
                {
                    customers.Add(customer);
                }
            }

            dataGridCustomers.ItemsSource = customers;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load customers:\n\n{ex.Message}",
                "Customer Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnAddCustomerAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Please enter a customer name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerPhone.Text))
            {
                MessageBox.Show("Please enter a customer phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerEmail.Text))
            {
                MessageBox.Show("Please enter a customer email.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await customerRepository.CreateAsync(new Customer
            {
                Name = txtCustomerName.Text,
                PhoneNumber = txtCustomerPhone.Text,
                Email = txtCustomerEmail.Text,
                Address = txtCustomerAddress.Text,
                Created = DateTime.Now
            });

            txtCustomerName.Clear();
            txtCustomerPhone.Clear();
            txtCustomerEmail.Clear();
            txtCustomerAddress.Clear();

            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async void BtnDeleteCustomerAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is Customer selectedCustomer)
            {
                await customerRepository.DeleteAsync(selectedCustomer);

                await LoadCustomersAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}