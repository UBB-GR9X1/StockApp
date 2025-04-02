// MainWindow.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using Alerts.Data;
using Alerts.Models;
using Alerts.ViewModels;

namespace Alerts
{
    public sealed partial class AlertWindow : Page
    {
        private ObservableCollection<Alert> alerts;

        public AlertWindow()
        {
            this.InitializeComponent();
            LoadAlerts();
        }

        // Load alerts from the database and bind to the ListView
        private void LoadAlerts()
        {
            using (var db = new StocksAppContext())
            {
                var alertList = db.Alerts.ToList();
                alerts = new ObservableCollection<Alert>(alertList);
                AlertsListView.ItemsSource = alerts;  // Bind to ListView
            }
        }

        // Create a new alert (add a new row to the ListView)
        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            var newAlert = new Alert
            {
                Name = "New Alert",
                UpperBound = 100,
                LowerBound = 0,
                ToggleOnOff = false
            };

            // Add new alert to the collection and ListView
            alerts.Add(newAlert);
            AlertsListView.SelectedItem = newAlert;
        }

        // Delete the selected alert from the ListView and database
        private async void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAlert = (Alert)AlertsListView.SelectedItem;
            if (selectedAlert != null)
            {
                // Remove from ObservableCollection first
                alerts.Remove(selectedAlert);

                using (var db = new StocksAppContext())
                {
                    // Only delete if the alert exists in the database (has a valid AlertId)
                    if (selectedAlert.AlertId != 0)
                    {
                        var alertToDelete = db.Alerts.FirstOrDefault(a => a.AlertId == selectedAlert.AlertId);
                        if (alertToDelete != null)
                        {
                            db.Alerts.Remove(alertToDelete);
                            db.SaveChanges();
                        }
                    }
                }

                // Show confirmation dialog after deleting
                var dialog = new ContentDialog
                {
                    Title = "Alert Deleted",
                    Content = "The selected alert has been successfully deleted.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot // Required in WinUI 3
                };
                await dialog.ShowAsync();
            }
            else
            {
                // Show validation error if no alert is selected
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please select an alert to delete.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot // Required in WinUI 3
                };
                await errorDialog.ShowAsync();
            }
        }

        // Save all changes from the ListView to the database
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new StocksAppContext())
            {
                foreach (var alert in alerts)
                {
                    if (alert.LowerBound > alert.UpperBound)
                    {
                        await ShowValidationError("Lower Bound cannot be greater than Upper Bound.");
                        return;
                    }

                    var existingAlert = db.Alerts.FirstOrDefault(a => a.AlertId == alert.AlertId);

                    if (existingAlert == null)
                    {
                        db.Alerts.Add(alert);
                    }
                    else
                    {
                        existingAlert.Name = alert.Name;
                        existingAlert.UpperBound = alert.UpperBound;
                        existingAlert.LowerBound = alert.LowerBound;
                        existingAlert.ToggleOnOff = alert.ToggleOnOff;
                    }
                }
                db.SaveChanges();

                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Alerts saved !",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot // Required in WinUI 3
                };
                await dialog.ShowAsync();
            }
        }

        // Show validation error message
        private async System.Threading.Tasks.Task ShowValidationError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Validation Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot // Required in WinUI 3
            };
            await dialog.ShowAsync();
        }

        private void LowerBound_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Alert alert)
            {
                if (int.TryParse(textBox.Text, out int lowerBound))
                {
                    // If the lower bound is greater than the upper bound, show an error but don't change the text
                    if (lowerBound > alert.UpperBound)
                    {
                        textBox.Text = alert.LowerBound.ToString(); // Reset to the previous valid value
                        ShowValidationError("Lower Bound cannot be greater than Upper Bound.");
                    }
                    else
                    {
                        alert.LowerBound = lowerBound; // Valid value, update the lower bound
                    }
                }
                else
                {
                    ShowValidationError("Please enter a valid number for Lower Bound.");
                }
            }
        }

        // Upper Bound LostFocus Event Handler
        private void UpperBound_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Alert alert)
            {
                if (int.TryParse(textBox.Text, out int upperBound))
                {
                    // If the upper bound is less than the lower bound, show an error but don't change the text
                    if (upperBound < alert.LowerBound)
                    {
                        textBox.Text = alert.UpperBound.ToString(); // Reset to the previous valid value
                        ShowValidationError("Upper Bound cannot be less than Lower Bound.");
                    }
                    else
                    {
                        alert.UpperBound = upperBound; // Valid value, update the upper bound
                    }
                }
                else
                {
                    ShowValidationError("Please enter a valid number for Upper Bound.");
                }
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        private async void LowerBound_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (!int.TryParse(textBox.Text, out int lowerBound))
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Input Error",
                        Content = "Please enter a valid number for Lower Bound.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot // Required in WinUI 3
                    };

                    // Ensure async operations are awaited properly
                    await dialog.ShowAsync();
                    textBox.Text = string.Empty; // Clear invalid input
                }
            }
        }

        private async void UpperBound_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (!int.TryParse(textBox.Text, out int upperBound))
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Input Error",
                        Content = "Please enter a valid number for Upper Bound.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot // Required in WinUI 3
                    };

                    // Use await properly to show the dialog asynchronously
                    await dialog.ShowAsync();
                    textBox.Text = string.Empty; // Clear invalid input
                }
            }
        }




    }
}
