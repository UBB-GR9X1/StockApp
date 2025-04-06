using Catel.MVVM;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using StockApp.Profile;
using StockApp.StockPage;
using StockNewsPage.Services;
using StockNewsPage.ViewModels;
using StocksHomepage.Model;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StocksApp
{
    public sealed partial class ProfilePage : Page
    {
        private ProfilePageViewModel viewModel;

        ICommand UpdateProfileButton { get; }

        public ProfilePage()
        {
            UpdateProfileButton = new RelayCommand(() => GoToUpdatePage());
        }

        private void doStuff()
        {
            this.InitializeComponent();
            this.showUserInformation();
            StocksListView.ItemsSource = viewModel.getUserStocks();

            if (viewModel.isHidden())
            {
                this.hideProfile();
            }

            userStocksShowUsername();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string authorCNP)
            {
                viewModel = new ProfilePageViewModel(authorCNP);
                this.DataContext = viewModel;
            }

            doStuff();
        }

        private void showUserInformation()
        {
            UsernameTextBlock.Text = viewModel.getUsername();
            ProfileDescription.Text = viewModel.getDescription();

            ProfileImage.Source = viewModel.ImageSource;
        }

        private void GoToUpdatePage()
        {
            StockNewsPage.Services.NavigationService.Instance.Navigate(typeof(UpdateProfilePage), viewModel.getLoggedInUserCNP());
        }

        private void getSelectedStock(object sender, RoutedEventArgs e)
        {
            string selectedStock = (string)StocksListView.SelectedItem;
            StockName.Text = selectedStock;
        }

        private void hideProfile()
        {
            StocksListView.Visibility = Visibility.Collapsed;
            ProfileDescription.Visibility = Visibility.Collapsed;
            ProfileImage.Visibility = Visibility.Collapsed;
            EnterStockButton.Visibility = Visibility.Collapsed;
        }

        public void goBack(object sender, RoutedEventArgs e)
        {
            // Go back to the previous page
            StockNewsPage.Services.NavigationService.Instance.GoBack();
        }

        public void userStocksShowUsername()
        {
            // Show the username in the user's stock list
            UsernameMyStocks.Text = viewModel.getUsername() + "'s STOCKS: ";
        }

        public void GoToStockButton(object sender, RoutedEventArgs e)
        {
            string selectedStock = (string)StocksListView.SelectedItem;
            string stockName = viewModel.extractMyStockName(selectedStock);
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
