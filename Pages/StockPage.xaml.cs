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

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockPage : Page
    {
        private StockPageViewModel viewModel;
        private Page previousPage;

        private ICommand command { get; }

        /// <summary>
        /// Constructor for the StockPage class.
        /// </summary>
        public StockPage(StockPageViewModel stockPageViewModel)
        {
            this.viewModel = stockPageViewModel;
            this.DataContext = this.viewModel;
            this.InitializeComponent();
            this.command = new StockNewsRelayCommand(() => this.AuthorButtonClick());
        }

        /// <summary>
        /// Handles the click event for the return button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReturnButtonClick(object sender, RoutedEventArgs e)
        {
            //NavigationService.Instance.GoBack();
            Frame rootFrame = App.MainAppWindow.MainAppFrame;
            rootFrame.Content = this.previousPage;
        }

        /// <summary>
        /// Handles the click event for the author button.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void AuthorButtonClick()
        {
            if (this.viewModel == null)
            {
                throw new InvalidOperationException("ViewModel is not initialized");
            }

            NavigationService.Instance.Navigate(typeof(ProfilePage), this.viewModel.GetStockAuthor());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Retrieve the stock name passed during navigation
            if (e.Parameter is StockDetailsDTO stockDetailsDTO)
            {
                this.viewModel.SelectedStock = stockDetailsDTO.StockDetails;
                this.previousPage = stockDetailsDTO.PreviousPage;
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
            this.viewModel.ToggleFavorite();
        }

        /// <summary>
        /// Handles the click event for the alerts button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AlertsButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException("Alerts feature is not implemented yet.");
            //NavigationService.Instance.Navigate(typeof(AlertsView), this.viewModel!.SelectedStock);
        }

        /// <summary>
        /// Handles the click event for the buy button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void BuyButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.viewModel?.IsGuest ?? true)
            {
                await this.ShowDialogAsync("Login Required", "You need to be logged in to buy stocks.");
                return;
            }

            int quantity = (int)this.QuantityInput.Value;
            bool success = await this.viewModel.BuyStock(quantity);
            this.QuantityInput.Value = 1;

            if (!success)
            {
                await this.ShowDialogAsync("Not enough gems", "You don't have enough gems to buy this stock.");
            }
        }

        /// <summary>
        /// Handles the click event for the sell button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void SellButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.viewModel?.IsGuest ?? true)
            {
                await this.ShowDialogAsync("Login Required", "You need to be logged in to sell stocks.");
                return;
            }

            int quantity = (int)this.QuantityInput.Value;
            bool success = await this.viewModel.SellStock(quantity);
            this.QuantityInput.Value = 1;

            if (!success)
            {
                await this.ShowDialogAsync("Not enough stocks", "You don't have enough stocks to sell.");
            }
        }

        private async Task ShowDialogAsync(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot,
            };
            await dialog.ShowAsync();
        }
    }
}
