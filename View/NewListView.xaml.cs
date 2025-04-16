using Microsoft.UI.Xaml.Controls;
using StockNewsPage.ViewModels;
using Microsoft.UI.Xaml.Input;
using StockApp.Service;

namespace StockNewsPage.Views
{
    public sealed partial class NewsListView : Page
    {
        public NewsListViewModel ViewModel { get; } = new NewsListViewModel();

        public NewsListView()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }

        private void OnRefreshContainerRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            ViewModel.RefreshCommand.Execute(null);
        }

        private void OnEscapeKeyInvoked(KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.ClearSearchCommand.Execute(null);
            args.Handled = true;
        }

        private void OnCategoryFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectedCategory != null)
            {
                ViewModel.RefreshCommand.Execute(null);
            }
        }

        //back button
        private void OnBackButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            NavigationService.Instance.GoBack();
        }
    }
}