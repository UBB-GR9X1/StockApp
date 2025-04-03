using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using StockApp.Profile;

namespace StocksApp
{
    public sealed partial class ProfilePage : Page
    {
        private ProfilePageViewModel viewModel = new ProfilePageViewModel();

        public ProfilePage()
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

        private void showUserInformation()
        {
            UsernameTextBlock.Text = viewModel.getUsername();
            ProfileDescription.Text = viewModel.getDescription();

            ProfileImage.Source = viewModel.ImageSource;
        }

        private void GoToUpdatePage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UpdateProfilePage));
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
            Frame.GoBack();
        }

        public void userStocksShowUsername()
        {
            // Show the username in the user's stock list
            UsernameMyStocks.Text = viewModel.getUsername() + "'s STOCKS: ";
        }
    }
}
