using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using GemStore.Models;
using GemStore.ViewModels;

namespace GemStore
{
    public sealed partial class GemStoreWindow : Page
    {
        private StoreViewModel viewModel;

        public GemStoreWindow()
        {
            this.InitializeComponent();
            viewModel = new StoreViewModel();
            gemDealsListView.DataContext = viewModel;
        }

        private async void OnBuyClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is GemDeal selectedDeal)
            {
                ComboBox bankAccountDropdown = new ComboBox
                {
                    ItemsSource = viewModel.GetUserBankAccounts(),
                    SelectedIndex = 0
                };

                StackPanel dialogContent = new StackPanel();
                dialogContent.Children.Add(new TextBlock { Text = $"You are about to buy {selectedDeal.GemAmount} Gems for {selectedDeal.Price}€.\n\nSelect a Bank Account:" });
                dialogContent.Children.Add(bankAccountDropdown);

                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirm Purchase",
                    Content = dialogContent,
                    PrimaryButtonText = "Buy",
                    CloseButtonText = "Cancel",
                    XamlRoot = rootGrid.XamlRoot
                };

                ContentDialogResult result = await confirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    string selectedAccount = bankAccountDropdown.SelectedItem.ToString();
                    string purchaseResult = viewModel.BuyGems(selectedDeal, selectedAccount);
                    ShowSuccessDialog(purchaseResult);
                }
            }
            else
            {
                ShowErrorDialog("Please select a deal before buying.");
            }
        }

        private async void ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = rootGrid.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        private async void ShowSuccessDialog(string message)
        {
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = rootGrid.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        private async void OnSellClicked(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(sellInput.Text, out int gemsToSell) || gemsToSell <= 0)
            {
                ShowErrorDialog("Enter a valid number of Gems.");
                return;
            }

            if (gemsToSell > viewModel.UserGems)
            {
                ShowErrorDialog("Not enough Gems to sell.");
                return;
            }

            ComboBox bankAccountDropdown = new ComboBox
            {
                ItemsSource = viewModel.GetUserBankAccounts(),
                SelectedIndex = 0
            };

            StackPanel dialogContent = new StackPanel();
            dialogContent.Children.Add(new TextBlock { Text = $"You are about to sell {gemsToSell} Gems for {gemsToSell / 100.0}€.\n\nSelect a Bank Account from below:\n" });
            dialogContent.Children.Add(bankAccountDropdown);

            ContentDialog sellDialog = new ContentDialog
            {
                Title = "Confirm Sale",
                Content = dialogContent,
                PrimaryButtonText = "Sell",
                CloseButtonText = "Cancel",
                XamlRoot = rootGrid.XamlRoot
            };

            ContentDialogResult result = await sellDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string selectedAccount = bankAccountDropdown.SelectedItem.ToString();
                string sellResult = viewModel.SellGems(gemsToSell, selectedAccount);
                ShowSuccessDialog(sellResult);
            }
        }

        private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            // TODO: Implement navigation to Home Page
        }
    }
}