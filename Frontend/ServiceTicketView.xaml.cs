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
public partial class ServiceTicketView : UserControl
{
    private readonly EquipmentRepository equipmentRepository;
    private readonly UserRepository userRepository;
    private readonly ServiceTicketRepository serviceTicketRepository;
    private readonly FaultRepository faultRepository;
    private ObservableCollection<ServiceTicket> serviceTickets = new ObservableCollection<ServiceTicket>();
    private ObservableCollection<Fault> faults = new ObservableCollection<Fault>();
    public ServiceTicketView(EquipmentRepository equipmentRepository, UserRepository userRepository, ServiceTicketRepository serviceTicketRepository, FaultRepository faultRepository)
    {
        InitializeComponent();
        this.equipmentRepository = equipmentRepository;
        this.userRepository = userRepository;
        this.serviceTicketRepository = serviceTicketRepository;
        this.faultRepository = faultRepository;

        Loaded += async (s, e) => await LoadServiceTicketsAsync();
    }

    private async Task LoadServiceTicketsAsync()
    {
        try
        {
            var _serviceTickets = await serviceTicketRepository.GetAllAsync();
            cmbUsers.ItemsSource = await userRepository.GetAllAsync();
            cmbEquipments.ItemsSource = await equipmentRepository.GetAllAsync();

            serviceTickets.Clear();

            if (_serviceTickets != null)
            {
                foreach (var ticket in _serviceTickets)
                {
                    serviceTickets.Add(ticket);
                }
            }

            cmbServiceTickets.ItemsSource = serviceTickets;

            dataGridServiceTickets.ItemsSource = serviceTickets;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load equipments:\n\n{ex.Message}",
                "Service Tickets Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnAddServiceTicketAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (cmbEquipments.SelectedValue is not int equipmentId)
            {
                MessageBox.Show("Please select an equipment.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbUsers.SelectedValue is not int userId)
            {
                MessageBox.Show("Please select an equipment.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await serviceTicketRepository.CreateAsync(new ServiceTicket
            {
                UserId = userId,
                EquipmentId = equipmentId,
                IntakeDate = DateTime.Now
            });

            await LoadServiceTicketsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async void BtnDeleteServiceTicketAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is ServiceTicket selectedServiceTicket)
            {
                await serviceTicketRepository.DeleteAsync(selectedServiceTicket);

                await LoadServiceTicketsAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async void BtnAddFaultAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (cmbServiceTickets.SelectedValue is not int serviceTicketId)
            {
                MessageBox.Show("Please select a service ticket.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await faultRepository.CreateAsync(new Fault
            {
                ServiceTicketId = serviceTicketId,
                FaultName = txtFaultName.Text ?? string.Empty,
                Diagnosis = txtDiagnosis.Text ?? string.Empty,
                Repairs = txtRepairs.Text ?? string.Empty
            });

            txtFaultName.Clear();
            txtDiagnosis.Clear();
            txtRepairs.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}