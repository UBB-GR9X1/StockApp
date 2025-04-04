using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StockApp.StockPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockPage : Page
    {
        private StockPageViewModel _viewModel;

        public StockPage()
        {
            this.InitializeComponent();
        }

        public void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        public void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            // this.Frame.Navigate(typeof(AuthorPage.AuthorPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Retrieve the stock name passed during navigation
            if (e.Parameter is string stockName)
            {
                _viewModel = new StockPageViewModel(stockName, PriceLabel, IncreaseLabel, OwnedStocks, StockChart);
                this.DataContext = _viewModel;
            }
        }

        public void FavoriteButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ToggleFavorite();
        }

        public void BuyButtonClick(object sender, RoutedEventArgs e) {
            int quantity = (int)QuantityInput.Value;
            bool r = _viewModel.BuyStock(quantity);
            QuantityInput.Value = 1;
            if (!r)
            {
                //var dialog = new ContentDialog
                //{
                //    Title = "Not enough gems",
                //    Content = "You don't have enough gems to buy this stock.",
                //    CloseButtonText = "OK"
                //};
                //dialog.ShowAsync();
                Console.WriteLine("Not enough Gems!");
            }
        }

        public void SellButtonClick(object sender, RoutedEventArgs e)
        {
            int quantity = (int)QuantityInput.Value;
            bool r = _viewModel.SellStock(quantity);
            QuantityInput.Value = 1;
            if (!r)
            {
                //var dialog = new ContentDialog
                //{
                //    Title = "Not enough stocks",
                //    Content = "You don't have enough stocks to sell.",
                //    CloseButtonText = "OK"
                //};

                //dialog.ShowAsync();
                Console.WriteLine("Not enough Stocks!");
            }
        }
    }
}
