using ISS_CreateStockPage.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StockApp.Views;
using System;
using System.Text.RegularExpressions;

namespace ISS_CreateStockPage
{
    public partial class MainWindow : Window
    {
        CreateStockViewModel viewModel;

        public MainWindow()
        {
            this.InitializeComponent();
            viewModel = new CreateStockViewModel();
        }

        private async void GoBack_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Navigation",
                Content = "Go back to the previous page.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async void CreateStock_Click(object sender, RoutedEventArgs e)
        {
            string stockName = StockNameTextBox.Text.Trim();
            string stockQuantityText = StockQuantityTextBox.Text.Trim();
            string stockSymbol = StockSymbolTextBox.Text.Trim();
            string authorCNP = AuthorCNPTextBox.Text.Trim();
            string stockPriceText = StockPriceTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(stockName) || string.IsNullOrWhiteSpace(stockQuantityText) ||
                string.IsNullOrWhiteSpace(stockSymbol) || string.IsNullOrWhiteSpace(authorCNP) ||
                string.IsNullOrWhiteSpace(stockPriceText))
            {
                await ShowDialog("All fields are mandatory!");
                return;
            }

            if (!float.TryParse(stockQuantityText, out float stockQuantity) || !float.TryParse(stockPriceText, out float stockPrice))
            {
                await ShowDialog("Stock Quantity and Stock Price must be valid numbers!");
                return;
            }

            Stock stock = new Stock(stockName, stockSymbol, stockQuantity, stockPrice, authorCNP);
            string errorMessage = viewModel.validateStock(stock);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await ShowDialog(errorMessage);
            }
            else
            {
                viewModel.AddStock(stock);
                await ShowDialog("Stock added successfully!");
            }

            StockNameTextBox.Text = "";
            StockQuantityTextBox.Text = "";
            StockSymbolTextBox.Text = "";
            AuthorCNPTextBox.Text = "";
            StockPriceTextBox.Text = "";
        }

        private async System.Threading.Tasks.Task ShowDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Notification",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}