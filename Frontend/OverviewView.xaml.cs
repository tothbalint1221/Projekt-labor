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
using ServiceManagerApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ServiceManagerApp.Services.Overview;
using System.Windows.Interop;

namespace ServiceManagerApp.Frontend
{
    public partial class OverviewView : UserControl
    {
        private readonly OverviewViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public OverviewView(OverviewViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            this._viewModel = viewModel;
            _serviceProvider = serviceProvider;
            DataContext = viewModel;

            Loaded += OverviewView_Loaded;
        }

        private async void OverviewView_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                await _viewModel.LoadOverviewAsync();

            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"Could not load overview:\n\n{ex.Message}",
                    "Overview load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );       
             }
        }

        private async void BtnDetails_Click(object sender, RoutedEventArgs eventArgs)
        {
            var button = (Button) sender;
            var summary = (TicketSummary) button.DataContext;

            var ticket = await _viewModel.GetTicketAsync(summary.Id);

            if(ticket is null)
            {
                MessageBox.Show("This ticket no longer exists.", "Ticket Not Found",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;

            }

            var detailsWindwo = ActivatorUtilities.CreateInstance<ServiceTicketDetails>
                (_serviceProvider, ticket);

            detailsWindwo.Owner = Window.GetWindow(this);
            detailsWindwo.ShowDialog();

            await _viewModel.LoadOverviewAsync();

        }


    }
}
