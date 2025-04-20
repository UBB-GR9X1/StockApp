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
        /// <summary>
        /// Initializes a new instance of the <see cref="HomepageView"/> class.
        /// </summary>
        public HomepageView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
        }

        /// <summary>
        /// Gets the view model for the homepage view.
        /// </summary>
        public HomepageViewModel ViewModel { get; } = new();

        /// <summary>
        /// Handles the click event for the stock item in the homepage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void GoToStock(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not HomepageStock selectedStock)
            {
                throw new InvalidOperationException("Clicked item is not a valid stock");
            }

            NavigationService.Initialize(new FrameAdapter(this.Frame));
            NavigationService.Instance.Navigate(typeof(StockPage), selectedStock.StockDetails);
        }
    }
}