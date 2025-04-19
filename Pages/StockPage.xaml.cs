namespace StockApp.Pages
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Services;
    using StockApp.ViewModels;
    using StockApp.Views;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockPage : Page
    {
        private StockPageViewModel? _viewModel;
        ICommand command { get; }

        /// <summary>
        /// Constructor for the StockPage class.
        /// </summary>
        public StockPage()
        {
            this.InitializeComponent();
            command = new StockNewsRelayCommand(() => AuthorButtonClick());
        }

        /// <summary>
        /// Handles the click event for the return button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReturnButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.GoBack();
        }

        /// <summary>
        /// Handles the click event for the author button.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void AuthorButtonClick()
        {
            if (_viewModel == null)
            {
                throw new InvalidOperationException("ViewModel is not initialized");
            }

            NavigationService.Instance.Navigate(typeof(ProfilePage), _viewModel.GetStockAuthor());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Retrieve the stock name passed during navigation
            if (e.Parameter is Stock stockName)
            {
                _viewModel = new StockPageViewModel(stockName, PriceLabel, IncreaseLabel, OwnedStocks, StockChart);
                this.DataContext = _viewModel;
            }
            else
            {
                throw new InvalidOperationException("Parameter is not of type Stock");
            }
        }

        /// <summary>
        /// Handles the click event for the favorite button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FavoriteButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ToggleFavorite();
        }

        /// <summary>
        /// Handles the click event for the alerts button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AlertsButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.Navigate(typeof(AlertsView));
        }

        /// <summary>
        /// Handles the click event for the buy button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void BuyButtonClick(object sender, RoutedEventArgs e)
        {
            int quantity = (int)QuantityInput.Value;
            bool success = _viewModel?.BuyStock(quantity) ?? false;
            QuantityInput.Value = 1;

            if (!success)
            {
                await ShowDialogAsync("Not enough gems", "You don't have enough gems to buy this stock.");
            }
        }

        /// <summary>
        /// Handles the click event for the sell button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void SellButtonClick(object sender, RoutedEventArgs e)
        {
            int quantity = (int)QuantityInput.Value;
            bool success = _viewModel?.SellStock(quantity) ?? false;
            QuantityInput.Value = 1;

            if (!success)
            {
                await ShowDialogAsync("Not enough stocks", "You don't have enough stocks to sell.");
            }
        }

        private async Task ShowDialogAsync(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
