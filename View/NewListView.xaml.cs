using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using StockApp.Service;
using StockApp.ViewModel;

namespace StockNewsPage.Views
{
    public sealed partial class NewsListView : Page
    {
        public NewsListViewModel ViewModel { get; } = new();

        public NewsListView()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }

        private void RefreshContainerRefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            ViewModel.RefreshCommand.Execute(null);
        }

        private void EscapeKeyInvoked(KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.ClearSearchCommand.Execute(null);
            args.Handled = true;
        }

        private void CategoryFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectedCategory != null)
            {
                ViewModel.RefreshCommand.Execute(null);
            }
        }

        //back button
        private void BackButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            NavigationService.Instance.GoBack();
        }
    }
}