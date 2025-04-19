namespace StockApp.Pages
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateStockPage : Page
    {
        public CreateStockPage()
        {
            this.InitializeComponent();
            this.DataContext = new CreateStockViewModel();
        }

        private void GoBackClick(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                throw new InvalidOperationException("No page to navigate back to.");
            }
        }
    }
}
