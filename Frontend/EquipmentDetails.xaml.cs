using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ServiceManagerApp.Repositories;

namespace ServiceManagerApp.Frontend;

public partial class EquipmentDetails : Window
{
    private readonly Equipment equipment;
    private readonly FaultRepository faultRepository;
    private readonly ObservableCollection<Fault> faults = new();

    public EquipmentDetails(Equipment equipment, FaultRepository faultRepository)
    {
        InitializeComponent();
        this.equipment = equipment;
        this.faultRepository = faultRepository;

        txtEquipmentId.Text = equipment.Id.ToString();
        txtCategory.Text = equipment.Category;
        txtBrand.Text = equipment.Brand;
        txtModel.Text = equipment.Model;
        txtSerialNumber.Text = equipment.SerialNumber;
        txtCustomerId.Text = equipment.CustomerId.ToString();
        dataGridFaults.ItemsSource = faults;

        Loaded += async (s, e) => await LoadFaultsAsync();
    }

    private async Task LoadFaultsAsync()
    {
        try
        {
            var equipmentFaults = await faultRepository.GetByEquipmentIdAsync(equipment.Id);
            faults.Clear();

            foreach (var fault in equipmentFaults)
            {
                faults.Add(fault);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load faults:\n\n{ex.Message}",
                "Fault Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
