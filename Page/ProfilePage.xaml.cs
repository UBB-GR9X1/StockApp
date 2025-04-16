using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using StockApp.Service;
using StockApp.StockPage;
using StockApp.ViewModel;
using StockNewsPage.ViewModels;

namespace StocksApp
{
    public sealed partial class ProfilePage : Page
    {
        private ProfilePageViewModel viewModel;

        ICommand UpdateProfileButton { get; }

        public ProfilePage()
        {
            UpdateProfileButton = new StockNewsRelayCommand(() => GoToUpdatePage());
        }

        private void DoStuff()
        {
            this.InitializeComponent();
            this.ShowUserInformation();
            StocksListView.ItemsSource = viewModel.GetUserStocks();

            if (viewModel.IsHidden())
            {
                this.HideProfile();
            }

            UserStocksShowUsername();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string authorCNP)
            {
                viewModel = new ProfilePageViewModel(authorCNP);
                this.DataContext = viewModel;
            }

            DoStuff();
        }

        private void ShowUserInformation()
        {
            UsernameTextBlock.Text = viewModel.GetUsername();
            ProfileDescription.Text = viewModel.GetDescription();

            ProfileImage.Source = viewModel.ImageSource;
        }

        private void GoToUpdatePage()
        {
            NavigationService.Instance.Navigate(typeof(UpdateProfilePage), viewModel.GetLoggedInUserCnp());
        }

        private void GetSelectedStock(object sender, RoutedEventArgs e)
        {
            string selectedStock = (string)StocksListView.SelectedItem;
            StockName.Text = selectedStock;
        }

        private void HideProfile()
        {
            StocksListView.Visibility = Visibility.Collapsed;
            ProfileDescription.Visibility = Visibility.Collapsed;
            ProfileImage.Visibility = Visibility.Collapsed;
            EnterStockButton.Visibility = Visibility.Collapsed;
        }

        public void GoBack(object sender, RoutedEventArgs e)
        {
            // Go back to the previous page
            NavigationService.Instance.GoBack();
        }

        public void UserStocksShowUsername()
        {
            // Show the username in the user's stock list
            UsernameMyStocks.Text = viewModel.GetUsername() + "'s STOCKS: ";
        }

        public void GoToStockButton(object sender, RoutedEventArgs e)
        {
            string selectedStock = (string)StocksListView.SelectedItem;
            string stockName = viewModel.ExtractMyStockName(selectedStock);
            if (stockName is string myStock)
            {
                NavigationService.Instance.Initialize(this.Frame);
                NavigationService.Instance.Navigate(typeof(StockPage), myStock);
            }
        }

        public void GoToStock(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is string myStock)
            {
                // this.Frame.Navigate(typeof(StockPage), null);
                NavigationService.Instance.Initialize(this.Frame);
                NavigationService.Instance.Navigate(typeof(StockPage), myStock);
            }
        }


    }
}
