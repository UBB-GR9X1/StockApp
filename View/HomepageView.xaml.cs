using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using StockNewsPage.Views;
using StocksApp;
using CreateStock;
using StockApp.StockPage;
using StockApp.Service;
using StockApp.Model;
using StockApp.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StocksHomepage
{
    public sealed partial class MainPage : Page
    {
        // add the view model as a property
        public HomepageViewModel ViewModel { get; }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new HomepageViewModel();
            DataContext = ViewModel;
        }

        private void OnFavoriteButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is HomepageStock stock)
            {
                ViewModel.FavoriteCommand.Execute(stock);
            }
        }

        private void OnCreateProfileClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as HomepageViewModel;

            if (viewModel != null)
            {
                viewModel.CreateUserProfile();
            }
        }

        private void OnSearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SearchQuery = SearchBox.Text;
        }

        private void OnSortDropdownSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortDropdown.SelectedItem is ComboBoxItem selectedItem)
            {
                ViewModel.SelectedSortOption = selectedItem.Content.ToString();
            }
        }

        public void GoToNews(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.Initialize(this.Frame);
            NavigationService.Instance.Navigate(typeof(NewsListView));
        }

        public void GoToCreateStock(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreateStockPage), null);
        }

        public void GoToHistory(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TransactionLog.TransactionLogView), null);
        }

        public void GoToProfile(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.Initialize(this.Frame);
            NavigationService.Instance.Navigate(typeof(ProfilePage), ViewModel.GetUserCnp());
        }

        public void GoToStore(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GemStore.GemStoreWindow), null);
        }

        public void GoToStock(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is HomepageStock stock)
            {
                NavigationService.Instance.Initialize(this.Frame);
                NavigationService.Instance.Navigate(typeof(StockPage), stock.Name);
            }
        }
    }
}
