namespace StockApp.Pages
{
    using System;
    using System.Windows.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Services;
    using StockApp.ViewModels;

    public sealed partial class ProfilePage : Page
    {
        private ProfilePageViewModel viewModel;

        ICommand UpdateProfileButton { get; }

        /// <summary>
        /// Constructor for the ProfilePage class.
        /// </summary>
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

            viewModel = new ProfilePageViewModel();
            this.DataContext = viewModel;
            //TODO:fix this naming jesus
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
            if (StocksListView.SelectedItem is string selectedStock)
            {
                StockName.Text = selectedStock;
            }
            else
            {
                StockName.Text = "No stock selected";
            }
        }

        private void HideProfile()
        {
            StocksListView.Visibility = Visibility.Collapsed;
            ProfileDescription.Visibility = Visibility.Collapsed;
            ProfileImage.Visibility = Visibility.Collapsed;
            EnterStockButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the click event for the "Go Back" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.GoBack();
        }

        /// <summary>
        /// Sets the text of the UsernameMyStocks TextBlock to the user's username.
        /// </summary>
        public void UserStocksShowUsername()
        {
            UsernameMyStocks.Text = viewModel.GetUsername() + "'s STOCKS: ";
        }

        /// <summary>
        /// Handles the click event for the "Go To Stock" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void GoToStockButton(object sender, RoutedEventArgs e)
        {
            if (this.StocksListView.SelectedItem is Stock selectedStock)
            {
                NavigationService.Initialize(new FrameAdapter(this.Frame));
                NavigationService.Instance.Navigate(typeof(StockPage), selectedStock);
            }
            else
            {
                throw new InvalidOperationException("No stock selected");
            }
        }
    }
}
