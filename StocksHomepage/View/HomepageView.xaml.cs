using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using StocksHomepage.ViewModel;
using StocksHomepage.Model;
using StockNewsPage.Services;
using StockNewsPage.Views;
using StocksApp;
using CreateStock;

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

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Stock stock)
            {
                ViewModel.FavoriteCommand.Execute(stock);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SearchQuery = SearchBox.Text;
        }

        private void SortDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            this.Frame.Navigate(typeof(ProfilePage), null);
        }

        public void GoToStore(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GemStore.GemStoreWindow), null);
        }

    }
}
