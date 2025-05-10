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
    using StockApp.Repositories;

    /// <summary>
    /// Represents the Profile Page of the application.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        private readonly ProfilePageViewModel viewModel;
        private ProfilePageViewModel viewModel;
        private static readonly IUserRepository userRepository = new UserRepository();

        /// <summary>
        /// Gets the command for updating the profile.
        /// </summary>
        private ICommand UpdateProfileButton { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model for the profile page.</param>
        public ProfilePage(ProfilePageViewModel viewModel)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.InitializeComponent();
            this.UpdateProfileButton = new StockNewsRelayCommand(() => this.GoToUpdatePage());
            this.viewModel = new ProfilePageViewModel();
            this.DataContext = this.viewModel;
        }

        /// <summary>
        /// Called when the page is navigated to.
        /// </summary>
        /// <param name="e">Event data for navigation.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                if (string.IsNullOrEmpty(userRepository.CurrentUserCNP))
                {
                    this.ShowNoUserMessage();
                    return;
                }

                this.LoadUserData();
            }
            catch (InvalidOperationException ex)
            {
                this.ShowNoUserMessage();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        private void ShowNoUserMessage()
        {
            this.UsernameTextBlock.Text = "No user logged in";
            this.ProfileDescription.Text = "Please create a profile first";
            this.StocksListView.ItemsSource = null;
            this.ProfileImage.Visibility = Visibility.Collapsed;
            this.EnterStockButton.Visibility = Visibility.Collapsed;
            this.updateWindowButton.Visibility = Visibility.Collapsed;
        }

        private void ShowErrorMessage(string message)
        {
            this.UsernameTextBlock.Text = "Error";
            this.ProfileDescription.Text = message;
            this.StocksListView.ItemsSource = null;
            this.ProfileImage.Visibility = Visibility.Collapsed;
            this.EnterStockButton.Visibility = Visibility.Collapsed;
            this.updateWindowButton.Visibility = Visibility.Collapsed;
        }

        private void LoadUserData()
        {
            try
            {
                // The view model properties will automatically update the UI through data binding
                if (this.viewModel.IsHidden())
                {
                    this.HideProfile();
                }

                // Update username in stocks section
                this.UserStocksShowUsername();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Navigates to the update profile page.
        /// </summary>
        private async void GoToUpdatePage()
        {
            if (this.viewModel == null)
            {
                this.ShowErrorMessage("No user profile available");
                return;
            }
            NavigationService.Instance.Navigate(typeof(UpdateProfilePage), this.viewModel.GetLoggedInUserCnp());
        }

        /// <summary>
        /// Retrieves the selected stock and displays its name.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GetSelectedStock(object sender, RoutedEventArgs e)
        {
            if (this.StocksListView.SelectedItem is Stock selectedStock)
            {
                this.StockName.Text = selectedStock.Name;
            }
            else
            {
                this.StockName.Text = "No stock selected";
            }
        }

        /// <summary>
        /// Hides the profile details from the UI.
        /// </summary>
        private void HideProfile()
        {
            this.StocksListView.Visibility = Visibility.Collapsed;
            this.ProfileDescription.Visibility = Visibility.Collapsed;
            this.ProfileImage.Visibility = Visibility.Collapsed;
            this.EnterStockButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the click event for the "Go Back" button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.GoBack();
        }

        /// <summary>
        /// Sets the text of the UsernameMyStocks TextBlock to the user's username.
        /// </summary>
        private async Task UserStocksShowUsernameAsync()
        {
            this.UsernameMyStocks.Text = this.viewModel.Username + "'s STOCKS: ";
        }

        /// <summary>
        /// Handles the click event for the "Go To Stock" button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <exception cref="InvalidOperationException">Thrown when no stock is selected.</exception>
        public void GoToStockButton(object sender, RoutedEventArgs e)
        {
            if (this.StocksListView.SelectedItem is Stock selectedStock)
            {
                NavigationService.Instance.Navigate(typeof(StockPage), selectedStock);
            }
            else
            {
                throw new InvalidOperationException("No stock selected");
            }
        }
    }
}
