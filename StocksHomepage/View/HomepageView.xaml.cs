using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using StocksHomepage.ViewModel;
using StocksHomepage.Model;

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
    }
}
