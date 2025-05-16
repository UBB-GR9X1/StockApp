namespace StockApp.Views
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class NewsArticleView : Page
    {
        private NewsDetailViewModel viewModel;

        /// <summary>
        /// Gets a new instance of the <see cref="NewsArticleView"/> class.
        /// </summary>
        public NewsDetailViewModel ViewModel
        {
            get => this.viewModel;
            set
            {
                this.viewModel = value;
                this.DataContext = value;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="NewsArticleView"/> class.
        /// </summary>
        public NewsArticleView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the click event for the related stock button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentException"></exception>
        private void RelatedStockClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
            {
                throw new ArgumentException("Sender is not a Button", nameof(sender));
            }

            if (button.Content is not string stockName)
            {
                throw new ArgumentException("Button content is not a valid stock name", nameof(sender));
            }
            //FIXME: navigate to the stock page
            throw new NotImplementedException("Not implemented");
        }
    }
}