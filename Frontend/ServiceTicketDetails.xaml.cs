using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using ServiceManagerApp.Repositories;
using ServiceManagerApp.Services.Users;
using System.Windows.Controls;

namespace ServiceManagerApp.Frontend;

public partial class ServiceTicketDetails : Window
{

    private readonly ServiceTicket serviceTicket;
    private readonly ObservableCollection<Fault> faults = new();
    private readonly ObservableCollection<TicketPart> ticketParts = new();
    private readonly ObservableCollection<string> changeLog = new();
    private readonly ServiceTicketRepository serviceTicketRepository;
    private readonly FaultRepository faultRepository;
    private readonly PartRepository partRepository;
    private readonly TicketPartRepository ticketPartRepository;
    private readonly UserRepository userRepository;
    private readonly UserSession userSession;
    private bool isInitializing = true;
    private bool isUpdatingStatus;
    private bool isUpdatingAssignment;

    public Status[] StatusOptions { get; } = Enum.GetValues<Status>();

    public ServiceTicketDetails(ServiceTicket serviceTicket, ServiceTicketRepository serviceTicketRepository, UserRepository userRepository, UserSession userSession, FaultRepository faultRepository, PartRepository partRepository, TicketPartRepository ticketPartRepository)
    {
        InitializeComponent();
        this.serviceTicket = serviceTicket;
        this.serviceTicketRepository = serviceTicketRepository;
        this.userRepository = userRepository;
        this.userSession = userSession;
        this.faultRepository = faultRepository;
        this.partRepository = partRepository;
        this.ticketPartRepository = ticketPartRepository;
        cmbAssignedTo.IsEnabled = CanChangeAssignment();

        foreach (var entry in serviceTicket.ChangeLog)
        {
            changeLog.Add(entry);
        }

        txtTicketId.Text = serviceTicket.Id.ToString();
        txtEquipment.Text = $"{serviceTicket.Equipment.Brand} {serviceTicket.Equipment.Model} ({serviceTicket.Equipment.SerialNumber})".Trim();
        txtCost.Text = serviceTicket.LaborCost.ToString("0.00", CultureInfo.CurrentCulture);
        cmbStatus.ItemsSource = Enum.GetValues<Status>();
        cmbStatus.SelectedItem = serviceTicket.Status;
        listboxChangeLog.ItemsSource = changeLog;
        UpdateAddButtons();
        isInitializing = false;

        Loaded += async (s, e) =>
        {
            await LoadFaultsAsync();
            await LoadTicketPartsAsync();
            await LoadPriceAsync();
            await LoadAssignedUsersAsync();
        };

    }

    private async Task LoadAssignedUsersAsync()
    {
        try
        {
            var users = await userRepository.GetAllAsync();
            var technicians = users.Where(user => user.Role == Role.Technician).ToList();

            isUpdatingAssignment = true;
            cmbAssignedTo.ItemsSource = technicians;
            cmbAssignedTo.SelectedValue = serviceTicket.UserId ?? 0;
            isUpdatingAssignment = false;
        }
        catch (Exception ex)
        {
            isUpdatingAssignment = false;
            MessageBox.Show(
                $"Could not load technicians:\n\n{ex.Message}",
                "Technician Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void CmbAssignedTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isInitializing || isUpdatingAssignment)
        {
            return;
        }

        if (!CanChangeAssignment())
        {
            cmbAssignedTo.SelectedValue = serviceTicket.UserId ?? 0;
            return;
        }

        if (cmbAssignedTo.SelectedValue is not int selectedUserId)
        {
            return;
        }

        var assignedUserId = selectedUserId == 0 ? (int?)null : selectedUserId;
        if (assignedUserId == serviceTicket.UserId)
        {
            return;
        }

        isUpdatingAssignment = true;
        cmbAssignedTo.IsEnabled = false;
        var previousUserId = serviceTicket.UserId;

        try
        {
            User? selectedUser = cmbAssignedTo.SelectedItem as User;
            var changeLogEntry = assignedUserId is null
                ? $"{DateTime.Now} - Ticket unassigned"
                : $"{DateTime.Now} - Assigned to: {selectedUser?.Name}";

            await serviceTicketRepository.UpdateAssignmentAsync(serviceTicket, assignedUserId, changeLogEntry);
            serviceTicket.UserId = assignedUserId;
            serviceTicket.User = assignedUserId is null ? null : selectedUser;
            changeLog.Add(changeLogEntry);
        }
        catch (Exception ex)
        {
            cmbAssignedTo.SelectedValue = previousUserId ?? 0;
            MessageBox.Show(
                $"Could not update the ticket assignment:\n\n{ex.Message}",
                "Assignment Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            cmbAssignedTo.IsEnabled = CanChangeAssignment();
            isUpdatingAssignment = false;
        }
    }

    private bool CanChangeAssignment() =>
        userSession.CurrentUser?.Role is Role.Admin or Role.Clerk;

    private async Task LoadFaultsAsync()
    {
        try
        {
            var ticketFaults = await faultRepository.GetByServiceTicketIdAsync(serviceTicket.Id);
            faults.Clear();

            foreach (var fault in ticketFaults)
            {
                faults.Add(fault);
            }

            dataGridFaults.ItemsSource = faults;
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

    private async Task LoadTicketPartsAsync()
    {
        try
        {
            var loadedTicketParts = await ticketPartRepository.GetByServiceTicketIdAsync(serviceTicket.Id);
            ticketParts.Clear();

            foreach (var ticketPart in loadedTicketParts)
            {
                ticketParts.Add(ticketPart);
            }

            dataGridTicketParts.ItemsSource = ticketParts;
            dataGridTicketParts.Items.Refresh();
            cmbParts.ItemsSource = await partRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load ticket parts:\n\n{ex.Message}",
                "Ticket Part Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async Task LoadPriceAsync()
    {
        try
        {
            serviceTicket.Price = await serviceTicketRepository.RecalculatePriceAsync(serviceTicket.Id);
            txtPrice.Text = serviceTicket.Price.ToString("C", CultureInfo.CurrentCulture);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load price:\n\n{ex.Message}",
                "Price Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnAddFaultAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtFaultName.Text))
            {
                MessageBox.Show("Please enter a fault name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await faultRepository.CreateAsync(new Fault
            {
                ServiceTicketId = serviceTicket.Id,
                FaultName = txtFaultName.Text.Trim(),
                Diagnosis = txtDiagnosis.Text.Trim(),
                Repairs = txtRepairs.Text.Trim()
            });

            txtFaultName.Clear();
            txtDiagnosis.Clear();
            txtRepairs.Clear();
            await LoadFaultsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not add the fault:\n\n{ex.Message}",
                "Fault Add Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private async void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isInitializing || isUpdatingStatus || cmbStatus.SelectedItem is not Status selectedStatus ||
            selectedStatus == serviceTicket.Status)
        {
            return;
        }

        isUpdatingStatus = true;
        cmbStatus.IsEnabled = false;
        var previousStatus = serviceTicket.Status;
        var previousCompletionDate = serviceTicket.CompletionDate;
        var previousChangeLogCount = serviceTicket.ChangeLog.Count;

        try
        {
            var changeLogEntry = $"{DateTime.Now} - Status changed to: {selectedStatus}";
            bool finishedToHandedover = (serviceTicket.Status == Status.Finished && selectedStatus == Status.HandedOver) ? true : false;
            serviceTicket.Status = selectedStatus;

            if (selectedStatus is Status.Finished or Status.HandedOver)
            {
                if (!finishedToHandedover)
                {
                    serviceTicket.CompletionDate = DateTime.Now;
                }
            } else
            {
                serviceTicket.CompletionDate = null;
            }
            await serviceTicketRepository.UpdateStatusAsync(serviceTicket, selectedStatus, changeLogEntry);
            changeLog.Add(changeLogEntry);
            UpdateAddButtons();
        }
        catch (Exception ex)
        {
            serviceTicket.Status = previousStatus;
            serviceTicket.CompletionDate = previousCompletionDate;
            while (serviceTicket.ChangeLog.Count > previousChangeLogCount)
            {
                serviceTicket.ChangeLog.RemoveAt(serviceTicket.ChangeLog.Count - 1);
            }
            cmbStatus.SelectedItem = previousStatus;
            UpdateAddButtons();
            MessageBox.Show(
                $"Could not update the service ticket status:\n\n{ex.Message}",
                "Status Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            cmbStatus.IsEnabled = true;
            isUpdatingStatus = false;
        }
    }

    private void UpdateAddButtons()
    {
        var canAddItems = serviceTicket.Status is not (Status.Finished or Status.HandedOver or Status.Cancelled);
        btnAddFault.IsEnabled = canAddItems;
        btnAddPart.IsEnabled = canAddItems;
    }

    private async void BtnDeleteFaultAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is Fault selectedFault)
            {
                await faultRepository.DeleteAsync(selectedFault);

                await LoadFaultsAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not delete the fault:\n\n{ex.Message}",
                "Fault Delete Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnIncrementQuantityAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is TicketPart selectedTicketPart)
            {   
                await ticketPartRepository.ChangeQuantityAsync(selectedTicketPart.Id, true);
                await LoadTicketPartsAsync();
                await LoadPriceAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not increase the ticket part quantity:\n\n{ex.Message}",
                "Ticket Part Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnDecrementQuantityAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is TicketPart selectedTicketPart)
            {   
                await ticketPartRepository.ChangeQuantityAsync(selectedTicketPart.Id, false);
                await LoadTicketPartsAsync();
                await LoadPriceAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not decrease the ticket part quantity:\n\n{ex.Message}",
                "Ticket Part Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnRemovePartFromTicketAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is TicketPart selectedTicketPart)
            {   
                await ticketPartRepository.DeleteAsync(selectedTicketPart);
                await LoadTicketPartsAsync();
                await LoadPriceAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not remove the ticket part:\n\n{ex.Message}",
                "Ticket Part Remove Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnAddTicketPart(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse(txtQuantity.Text, out var quantity) || quantity < 1)
            {
                MessageBox.Show("Enter a quantity greater than zero.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbParts.SelectedValue is not int partId)
            {
                MessageBox.Show("Please select a part.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var part = await partRepository.GetAsync(partId);

            if (part is null)
            {
                MessageBox.Show("Part not found.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await ticketPartRepository.CreateAsync(new TicketPart
            {
                ServiceTicketId = serviceTicket.Id,
                PartId = partId,
                Quantity = quantity,
                UnitPrice = part.PartCost
            });

            txtQuantity.Clear();
            cmbParts.SelectedIndex = -1;
            await LoadTicketPartsAsync();
            await LoadPriceAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not add the part:\n\n{ex.Message}",
                "Ticket Part Add Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnSaveLaborCost_Click(object sender, RoutedEventArgs e)
    {
        if (!decimal.TryParse(txtCost.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var laborCost) ||
            laborCost < 0)
        {
            MessageBox.Show(
                "Enter a valid non-negative labor cost.",
                "Validation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            txtCost.Text = serviceTicket.LaborCost.ToString("0.00", CultureInfo.CurrentCulture);
            return;
        }

        if (laborCost == serviceTicket.LaborCost)
        {
            return;
        }

        var previousLaborCost = serviceTicket.LaborCost;
        txtCost.IsEnabled = false;

        try
        {
            await serviceTicketRepository.UpdateLaborCostAsync(serviceTicket, laborCost);
            serviceTicket.LaborCost = laborCost;
            txtCost.Text = laborCost.ToString("0.00", CultureInfo.CurrentCulture);
            await LoadPriceAsync();
        }
        catch (Exception ex)
        {
            serviceTicket.LaborCost = previousLaborCost;
            txtCost.Text = previousLaborCost.ToString("0.00", CultureInfo.CurrentCulture);
            MessageBox.Show(
                $"Could not update the labor cost:\n\n{ex.Message}",
                "Labor Cost Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            txtCost.IsEnabled = true;
        }
    }
}
