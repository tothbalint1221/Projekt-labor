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

namespace ServiceManagerApp.Frontend
{
    public partial class OverviewView : UserControl
    {
        private readonly OverviewViewModel _viewModel;

        public OverviewView(OverviewViewModel viewModel)
        {
            InitializeComponent();

            this._viewModel = viewModel;
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

    }
}
