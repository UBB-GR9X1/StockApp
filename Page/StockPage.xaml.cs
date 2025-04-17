using System;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using StockApp.Command;
using StockApp.Service;
using StockApp.ViewModel;
using StocksApp;

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

        ICommand command { get; }


        public StockPage()
        {
            this.InitializeComponent();
            command = new StockNewsRelayCommand(() => AuthorButtonClick());
        }

        public void ReturnButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.GoBack();
        }


        public void AuthorButtonClick()
        {
            if (_viewModel == null)
                throw new InvalidOperationException("ViewModel is not initialized");

            NavigationService.Instance.Navigate(typeof(ProfilePage), _viewModel.GetStockAuthor());
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

        public void AlertsButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.Navigate(typeof(Alerts.AlertWindow));
        }

        public void BuyButtonClick(object sender, RoutedEventArgs e)
        {
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
