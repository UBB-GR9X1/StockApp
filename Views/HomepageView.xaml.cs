namespace StockApp.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Models;
    using StockApp.Services;
    using StockApp.ViewModels;
    using StockApp.Pages;

    public sealed partial class HomepageView : Page
    {
        // add the view model as a property
        public HomepageViewModel ViewModel { get; }

        public HomepageView()
        {
            this.InitializeComponent();
            ViewModel = new HomepageViewModel();
            DataContext = ViewModel;
        }

        private void FavoriteButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is HomepageStock stock)
            {
                ViewModel.FavoriteCommand.Execute(stock);
            }
        }

        private void CreateProfileClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as HomepageViewModel;

            if (viewModel != null)
            {
                viewModel.CreateUserProfile();
            }
        }

        private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SearchQuery = SearchBox.Text;
        }

        private void SortDropdownSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortDropdown.SelectedItem is ComboBoxItem selectedItem)
            {
                ViewModel.SelectedSortOption = selectedItem.Content.ToString();
            }
        }

        public void GoToNews(object sender, RoutedEventArgs e)
        {
            NavigationService.Initialize(this.Frame);
            NavigationService.Instance.Navigate(typeof(NewListView));
        }

        public void GoToCreateStock(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StockApp.Pages.CreateStockPage), null);
        }

        public void GoToHistory(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TransactionLogView), null);
        }

        public void GoToProfile(object sender, RoutedEventArgs e)
        {
            NavigationService.Initialize(this.Frame);
            NavigationService.Instance.Navigate(typeof(StockApp.Pages.ProfilePage), ViewModel.GetUserCNP());
        }

        public void GoToStore(object sender, RoutedEventArgs e)
        {
            NavigationService.Initialize(this.Frame);
            NavigationService.Instance.Navigate(typeof(StockApp.Pages.GemStoreWindow), null);
        }

        public void GoToStock(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is HomepageStock stock)
            {
                // this.Frame.Navigate(typeof(StockPage), null);
                NavigationService.Initialize(this.Frame);
                NavigationService.Instance.Navigate(typeof(StockPage), stock.Name);
            }
        }
    }
}
