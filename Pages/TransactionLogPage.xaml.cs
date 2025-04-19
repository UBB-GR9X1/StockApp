namespace StockApp.Pages
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Repositories;
    using StockApp.Services;
    using StockApp.ViewModels;

    /// <summary>
    /// A page that displays transaction log data.
    /// </summary>
    public sealed partial class TransactionLogPage : Page
    {
        private readonly TransactionLogViewModel viewModel;

        public TransactionLogPage()
        {
            this.InitializeComponent();
            viewModel = new(new TransactionLogService(new TransactionRepository()));

            // Set the DataContext for binding to the ViewModel
            this.DataContext = viewModel;

            // Event to show a message box (if requested by the ViewModel)
            viewModel.ShowMessageBoxRequested += ShowMessageBoxRequested;
        }

        // Event handler when the start date changes
        private void StartDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Trigger the load operation when the start date changes
            viewModel.StartDate = e.NewDate.Date;
            viewModel.LoadTransactions();
        }

        // Event handler when the end date changes
        private void EndDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Trigger the load operation when the end date changes
            viewModel.EndDate = e.NewDate.Date;
            viewModel.LoadTransactions();
        }

        // Event handler to show a message box when requested by the ViewModel
        private async void ShowMessageBoxRequested(string title, string content)
        {
            // Ensure we're using Window.Current after the window is fully loaded
            if (Window.Current != null)
            {
                // Create and show the ContentDialog
                var messageDialog = new ContentDialog
                {
                    Title = title,
                    Content = content,
                    CloseButtonText = "OK",
                    XamlRoot = Window.Current.Content.XamlRoot // Set the correct XamlRoot
                };

                await messageDialog.ShowAsync();
            }
            else
            {
                throw new InvalidOperationException("Window.Current is null. Cannot show the message box.");
            }
        }

        // Ensure the window is fully loaded before accessing Window.Current
        private void PageActivated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs e)
        {
            if (Window.Current != null)
            {
                System.Diagnostics.Debug.WriteLine("Page is now activated and Window.Current is available.");
            }
            else
            {
                throw new InvalidOperationException("Window.Current is null during page activation.");
            }
        }

        public void GoBack(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
