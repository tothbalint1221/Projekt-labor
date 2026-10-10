using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceManagerApp.Frontend;

public partial class PrimaryWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public PrimaryWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        NavigateTo("Customers");
    }

    private void BtnNav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string viewName)
        {
            NavigateTo(viewName);
        }
    }

    private void NavigateTo(string viewName)
    {
        switch (viewName)
        {
            case "Customers":
                MainContentArea.Content = _serviceProvider.GetRequiredService<CustomerView>();
                break;

            case "Equipments":
                MainContentArea.Content = _serviceProvider.GetRequiredService<EquipmentView>();
                break;

            case "Tickets":
                MainContentArea.Content = _serviceProvider.GetRequiredService<ServiceTicketView>();
                break;

            default:
                break;
        }
    }
}