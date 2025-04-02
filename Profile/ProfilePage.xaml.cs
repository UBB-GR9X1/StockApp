using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            if(viewModel.isHidden() == true)
            {
                this.hideProfile();
            }
            userStocksShowUsername();
        }

        private void showUserInformation()
        {
            UsernameTextBlock.Text = viewModel.getUsername();
            ProfileImage.Text = viewModel.getImage();
            ProfileDescription.Text = "DESCRIPTION: " + viewModel.getDescription();
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
            Frame.GoBack();
        }

        public void userStocksShowUsername()
        {
            UsernameMyStocks.Text = viewModel.getUsername() + "'s STOCKS: ";
        }
    }
}
