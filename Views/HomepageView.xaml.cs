namespace StockApp.Views
{
    using System;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Models;
    using StockApp.Pages;
    using StockApp.Services;
    using StockApp.ViewModels;

    public sealed partial class HomepageView : Page
    {
        public HomepageView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
        }

        public HomepageViewModel ViewModel { get; } = new();

        public void GoToStock(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not Stock myStock)
            {
                throw new InvalidOperationException("Clicked item is not a valid stock");
            }

            NavigationService.Initialize(new FrameAdapter(this.Frame));
            NavigationService.Instance.Navigate(typeof(StockPage), myStock);
        }
    }
}