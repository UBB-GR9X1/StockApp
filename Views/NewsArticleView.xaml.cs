namespace StockApp.Views
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using StockApp.Pages;
    using StockApp.Services;
    using StockApp.ViewModels;

    public sealed partial class NewsArticleView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewsArticleView"/> class.
        /// </summary>
        public NewsDetailViewModel ViewModel { get; } = new();

        /// <summary>
        /// The ID of the selected article.
        /// </summary>
        private string selectedArticleId;

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
                throw new ArgumentException("Sender is not a Button", nameof(sender));

            if (button.Content is not string stockName)
                throw new ArgumentException("Button content is not a valid stock name", nameof(sender));

            NavigationService.Instance.Navigate(typeof(StockPage), stockName);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not string articleId)
                throw new ArgumentException("Navigation parameter is not a valid article ID", nameof(e));

            this.ViewModel.LoadArticle(articleId);
        }
    }
}