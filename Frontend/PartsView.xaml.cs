using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ServiceManagerApp.Repositories;

namespace ServiceManagerApp;

public partial class PartsView : UserControl
{
    private readonly PartRepository partRepository;
    private readonly ObservableCollection<Part> parts = new();

    public PartsView(PartRepository partRepository)
    {
        InitializeComponent();
        this.partRepository = partRepository;

        Loaded += async (s, e) => await LoadPartsAsync();
    }

    private async Task LoadPartsAsync()
    {
        try
        {
            var loadedParts = await partRepository.GetAllAsync();

            parts.Clear();
            foreach (var part in loadedParts)
            {
                parts.Add(part);
            }

            dataGridParts.ItemsSource = parts;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not load parts:\n\n{ex.Message}",
                "Parts Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnAddPartAsync(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtPartName.Text))
        {
            MessageBox.Show("Please enter a part name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(txtPartQuantity.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var quantity) || quantity < 0)
        {
            MessageBox.Show("Please enter a valid non-negative quantity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(txtPartCost.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var partCost) || partCost < 0)
        {
            MessageBox.Show("Please enter a valid non-negative cost.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await partRepository.CreateAsync(new Part
            {
                PartName = txtPartName.Text.Trim(),
                Quantity = quantity,
                PartCost = partCost
            });

            txtPartName.Clear();
            txtPartQuantity.Clear();
            txtPartCost.Clear();

            await LoadPartsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not add part:\n\n{ex.Message}",
                "Part Save Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnDeletePartAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is Part selectedPart)
            {
                await partRepository.DeleteAsync(selectedPart);
                await LoadPartsAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not delete part:\n\n{ex.Message}",
                "Part Delete Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        try
        {
            if (e.EditAction != DataGridEditAction.Commit || e.Row.Item is not Part part)
            {
                return;
            }

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            await partRepository.UpdateAsync(part);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not update part:\n\n{ex.Message}",
                "Part Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}