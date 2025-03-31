using Microsoft.UI.Xaml.Controls;
using StockNewsPage.ViewModels;
using Microsoft.UI.Xaml.Input;

namespace StockNewsPage.Views
{
    public sealed partial class NewsListView : Page
    {
        public NewsListViewModel ViewModel { get; } = new NewsListViewModel();

        public NewsListView()
        {
            this.InitializeComponent();
            this.Loaded += NewsListView_Loaded;
        }

        private void NewsListView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }

        private void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            ViewModel.RefreshCommand.Execute(null);
        }

        private void EscapeKey_Invoked(KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.ClearSearchCommand.Execute(null);
            args.Handled = true;
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectedCategory != null)
            {
                ViewModel.RefreshCommand.Execute(null);
            }
        }
    }
}