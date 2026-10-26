using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using ServiceManagerApp.Repositories;
using System.Windows.Controls;

namespace ServiceManagerApp.Frontend;

public partial class ServiceTicketDetails : Window
{
    private readonly ServiceTicket serviceTicket;
    private readonly ObservableCollection<Fault> faults = new();
    private readonly ObservableCollection<TicketPart> ticketParts = new();
    private readonly ServiceTicketRepository serviceTicketRepository;
    private readonly FaultRepository faultRepository;
    private readonly PartRepository partRepository;
    private readonly TicketPartRepository ticketPartRepository;
    private readonly HashSet<ComboBox> updatingFaultStatuses = new();
    private bool isInitializing = true;
    private bool isUpdatingStatus;

    public Status[] StatusOptions { get; } = Enum.GetValues<Status>();

    public ServiceTicketDetails(ServiceTicket serviceTicket, ServiceTicketRepository serviceTicketRepository, FaultRepository faultRepository, PartRepository partRepository, TicketPartRepository ticketPartRepository)
    {
        InitializeComponent();
        this.serviceTicket = serviceTicket;
        this.serviceTicketRepository = serviceTicketRepository;
        this.faultRepository = faultRepository;
        this.partRepository = partRepository;
        this.ticketPartRepository = ticketPartRepository;

        txtTicketId.Text = serviceTicket.Id.ToString();
        txtEquipment.Text = $"{serviceTicket.Equipment.Brand} {serviceTicket.Equipment.Model} ({serviceTicket.Equipment.SerialNumber})".Trim();
        txtAssignedTo.Text = serviceTicket.User?.Name ?? "Unassigned";
        txtCost.Text = serviceTicket.LaborCost.ToString("0.00", CultureInfo.CurrentCulture);
        cmbStatus.ItemsSource = Enum.GetValues<Status>();
        cmbStatus.SelectedItem = serviceTicket.Status;
        UpdateAddButtons();
        isInitializing = false;

        Loaded += async (s, e) =>
        {
            await LoadFaultsAsync();
            await LoadTicketPartsAsync();
            await LoadPriceAsync();
        };

    }


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

        try
        {
            await serviceTicketRepository.UpdateStatusAsync(serviceTicket, selectedStatus);
            serviceTicket.Status = selectedStatus;
            UpdateAddButtons();

            if (selectedStatus == Status.Finished)
            {
                serviceTicket.CompletionDate = DateTime.Now;
            } else
            {
                serviceTicket.CompletionDate = null;
            }
        }
        catch (Exception ex)
        {
            cmbStatus.SelectedItem = serviceTicket.Status;
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
        var canAddItems = serviceTicket.Status != Status.Finished;
        btnAddFault.IsEnabled = canAddItems;
        btnAddPart.IsEnabled = canAddItems;
    }

    private async void CmbFaultStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox ||
            updatingFaultStatuses.Contains(comboBox) ||
            comboBox.DataContext is not Fault fault ||
            e.AddedItems.Count != 1 ||
            e.AddedItems[0] is not Status selectedStatus ||
            e.RemovedItems.Count != 1 ||
            e.RemovedItems[0] is not Status previousStatus ||
            selectedStatus == previousStatus)
        {
            return;
        }

        updatingFaultStatuses.Add(comboBox);
        comboBox.IsEnabled = false;

        try
        {
            await faultRepository.UpdateAsync(fault);
            fault.Status = selectedStatus;
        }
        catch (Exception ex)
        {
            fault.Status = previousStatus;
            comboBox.SelectedItem = previousStatus;
            MessageBox.Show(
                $"Could not update the fault status:\n\n{ex.Message}",
                "Fault Status Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            comboBox.IsEnabled = true;
            updatingFaultStatuses.Remove(comboBox);
        }
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
