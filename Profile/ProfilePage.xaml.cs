using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StocksApp.Services;

namespace StocksApp
{
    public sealed partial class ProfilePage : Page
    {
        private ProfieServices profServ = ProfieServices.Instance;

        public ProfilePage()
        {
            this.InitializeComponent();
            this.showUserInformation();
            StocksListView.ItemsSource = profServ.getUserStocks();
            if(profServ.isHidden() == true)
            {
                this.hideProfile();
            }
        }

        private void showUserInformation()
        {
            UsernameTextBlock.Text = profServ.getUsername();
            ProfileImage.Text = profServ.getImage();
            ProfileDescription.Text = "DESCRIPTION: " + profServ.getDescription();
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
    }
}
