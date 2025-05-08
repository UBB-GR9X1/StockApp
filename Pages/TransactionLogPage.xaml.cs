﻿namespace StockApp.Pages
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogPage"/> class.
        /// </summary>
        public TransactionLogPage()
        {
            this.InitializeComponent();
            //this.viewModel = new TransactionLogViewModel();

            // Set the DataContext for binding to the ViewModel
            this.DataContext = this.viewModel;

            // Event to show a message box (if requested by the ViewModel)
            this.viewModel.ShowMessageBoxRequested += this.ShowMessageBoxRequested;
        }

        // Event handler to show a message box when requested by the ViewModel
        private async void ShowMessageBoxRequested(string title, string content)
        {
            // Ensure we're using Window.Current after the window is fully loaded
            // Create and show the ContentDialog
            var messageDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = App.CurrentWindow.Content.XamlRoot, // Set the correct XamlRoot
            };

            await messageDialog.ShowAsync();
        }

        public void OnEndDateChanged(object sender, DatePickerSelectedValueChangedEventArgs e)
        {
            if (this.DataContext is TransactionLogViewModel viewModel)
            {
                if (sender is DatePicker datePicker && datePicker.SelectedDate.HasValue)
                {
                    viewModel.EndDate = datePicker.SelectedDate.Value.DateTime;
                }
            }
        }

        public void OnStartDateChanged(object sender, DatePickerSelectedValueChangedEventArgs e)
        {
            if (this.DataContext is TransactionLogViewModel viewModel)
            {
                if (sender is DatePicker datePicker && datePicker.SelectedDate.HasValue)
                {
                    viewModel.StartDate = datePicker.SelectedDate.Value.DateTime;
                }
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

    }
}
