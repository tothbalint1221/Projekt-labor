using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.IdentityModel.Tokens;
using ServiceManagerApp.Frontend;
using ServiceManagerApp.Repositories;
using ServiceManagerApp.Services.Users;

namespace ServiceManagerApp;

/// <summary>
/// Interaction logic for ServiceTicketView.xaml
/// </summary>
public partial class ServiceTicketView : UserControl
{
    private readonly EquipmentRepository equipmentRepository;
    private readonly UserRepository userRepository;
    private readonly UserSession userSession;
    private readonly ServiceTicketRepository serviceTicketRepository;
    private readonly FaultRepository faultRepository;
    private readonly PartRepository partRepository;
    private readonly TicketPartRepository ticketPartRepository;
    private ObservableCollection<ServiceTicket> serviceTickets = new ObservableCollection<ServiceTicket>();
    public ServiceTicketView(EquipmentRepository equipmentRepository, UserRepository userRepository, UserSession userSession, ServiceTicketRepository serviceTicketRepository, FaultRepository faultRepository, PartRepository partRepository, TicketPartRepository ticketPartRepository)
    {
        InitializeComponent();
        this.equipmentRepository = equipmentRepository;
        this.userRepository = userRepository;
        this.userSession = userSession;
        this.serviceTicketRepository = serviceTicketRepository;
        this.faultRepository = faultRepository;
        this.partRepository = partRepository;
        this.ticketPartRepository = ticketPartRepository;

        Loaded += async (s, e) => await LoadServiceTicketsAsync();
    }

    private async Task LoadServiceTicketsAsync()
    {
        try
        {
            var _serviceTickets = await serviceTicketRepository.GetAllAsync();
            cmbEquipments.ItemsSource = await equipmentRepository.GetAllAsync();

            serviceTickets.Clear();

            if (_serviceTickets != null)
            {
                foreach (var ticket in _serviceTickets)
                {
                    serviceTickets.Add(ticket);
                }
            }

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

            await serviceTicketRepository.CreateAsync(new ServiceTicket
            {
                EquipmentId = equipmentId,
                IntakeDate = DateTime.Now,
                ChangeLog = [$"{DateTime.Now} - Ticket Created"]
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

    private void BtnServiceTicketDetails(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is ServiceTicket selectedServiceTicket)
        {
            var detailsWindow = new ServiceTicketDetails(selectedServiceTicket, serviceTicketRepository, userRepository, userSession, faultRepository, partRepository, ticketPartRepository)
            {
                Owner = Window.GetWindow(this)
            };
            detailsWindow.ShowDialog();
            dataGridServiceTickets.Items.Refresh();
        }
    }

    private async void BtnSendEmailAsync(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not ServiceTicket selectedServiceTicket)
        {
            return;
        }

        if (selectedServiceTicket.Status != Status.Finished)
        {
            MessageBox.Show(
                "An email can only be sent when the service ticket is Finished.",
                "Email Send Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var customer = selectedServiceTicket.Equipment.Customer;
        if (string.IsNullOrWhiteSpace(customer?.Email))
        {
            MessageBox.Show(
                "The customer does not have an email address.",
                "Email Send Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var equipmentName = $"{selectedServiceTicket.Equipment.Brand} {selectedServiceTicket.Equipment.Model} ({selectedServiceTicket.Equipment.SerialNumber})".Trim();
        var emailSender = new EmailSender(
            customer.Name,
            customer.Email,
            equipmentName,
            selectedServiceTicket.Price);

        button.IsEnabled = false;
        try
        {
            await emailSender.SendAsync();
            var changeLogEntry = $"{DateTime.Now} - Email sent to: {customer.Email}";
            await serviceTicketRepository.AddEmailSentLogAsync(selectedServiceTicket, changeLogEntry);
            MessageBox.Show(
                "The email was sent successfully.",
                "Email Sent",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not send the email:\n\n{ex.Message}",
                "Email Send Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            button.IsEnabled = true;
        }
    }

}