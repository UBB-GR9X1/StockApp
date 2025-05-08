namespace StockApp.Views
{
    using System;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Models;
    using StockApp.Pages;
    using StockApp.ViewModels;

    public sealed partial class HomepageView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomepageView"/> class.
        /// </summary>
        public HomepageView(HomepageViewModel homepageViewModel)
        {
            this.InitializeComponent();
            this.DataContext = homepageViewModel;
            this.ViewModel = homepageViewModel;
        }

        /// <summary>
        /// Gets the view model for the homepage view.
        /// </summary>
        public HomepageViewModel ViewModel { get; }

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
            // Navigate to the stock page using the selected stock's name
            if (App.MainAppWindow != null && App.MainAppWindow.MainAppFrame.Content is Page currentPage)
            {
                StockDetailsDTO stockDetailsDTO =
                    new StockDetailsDTO(selectedStock.StockDetails, currentPage);
                App.MainAppWindow.MainAppFrame.Navigate(typeof(StockPage), stockDetailsDTO);
            }
        }
    }
}