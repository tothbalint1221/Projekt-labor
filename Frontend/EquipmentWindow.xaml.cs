using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.IdentityModel.Tokens;
using ServiceManagerApp.Repositories;

namespace ServiceManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class EquipmentWindow : Window
{
    private readonly EquipmentRepository equipmentRepository;
    private readonly CustomerRepository customerRepository;
    private ObservableCollection<Equipment> equipments = new ObservableCollection<Equipment>();
    public EquipmentWindow(EquipmentRepository equipmentRepository, CustomerRepository customerRepository)
    {
        InitializeComponent();
        this.equipmentRepository = equipmentRepository;
        this.customerRepository = customerRepository;

        Loaded += async (s, e) => await LoadEquipmentsAsync();
    }

    private async Task LoadEquipmentsAsync()
    {
        try
        {
            var _equipments = await equipmentRepository.GetAllAsync();
            cmbCustomers.ItemsSource = await customerRepository.GetAllAsync();

            equipments.Clear();

            if (_equipments != null)
            {
                foreach (var customer in _equipments)
                {
                    equipments.Add(customer);
                }
            }

            dataGridEquipments.ItemsSource = equipments;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load equipments:\n\n{ex.Message}",
                "Equipments Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnAddEquipmentAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Please enter a model name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSerialNumber.Text))
            {
                MessageBox.Show("Please enter a serial number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbCustomers.SelectedValue is not int customerId)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await equipmentRepository.CreateAsync(new Equipment
            {
                Category = txtCategory.Text ?? string.Empty,
                Brand = txtBrand.Text ?? string.Empty,
                Model = txtModel.Text,
                SerialNumber = txtSerialNumber.Text,
                CustomerId = customerId
            });

            txtCategory?.Clear();
            txtBrand?.Clear();
            txtModel.Clear();
            txtSerialNumber.Clear();

            await LoadEquipmentsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async void BtnDeleteEquipmentAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is Equipment selectedEquipment)
            {
                await equipmentRepository.DeleteAsync(selectedEquipment);

                await LoadEquipmentsAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}