using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Repository;
using Models;
using System;
using Models;

namespace Alerts
{
    public sealed partial class AlertWindow : Page
    {
        private readonly AlertRepository _alertRepository = new AlertRepository();
        private ObservableCollection<Alert> _alerts;

        public AlertWindow()
        {
            this.InitializeComponent();
            LoadAlerts();
        }

        private void LoadAlerts()
        {
            _alerts = new ObservableCollection<Alert>(_alertRepository.GetAllAlerts());
            AlertsListView.ItemsSource = _alerts;
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            var newAlert = new Alert
            {
                StockName = "New Stock",
                Name = "New Alert",
                UpperBound = 100,
                LowerBound = 0,
                ToggleOnOff = true
            };

            _alertRepository.AddAlert(newAlert);
            _alerts.Add(newAlert);
            AlertsListView.SelectedItem = newAlert;
        }

        private async void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlertsListView.SelectedItem is Alert selectedAlert)
            {
                _alertRepository.DeleteAlert(selectedAlert.AlertId);
                _alerts.Remove(selectedAlert);

                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Alert deleted successfully",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please select an alert to delete",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var alert in _alerts)
                {
                    if (alert.LowerBound > alert.UpperBound)
                    {
                        throw new Exception("Lower bound cannot be greater than upper bound");
                    }
                    _alertRepository.UpdateAlert(alert);
                }

                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "All alerts saved successfully",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}