namespace StockApp.Views
{
    using System;
    using System.Linq;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using StockApp.Services;
    using StockApp.ViewModels;
    using StockApp.Pages;

    public sealed partial class NewsArticleView : Page
    {
        public NewsDetailViewModel ViewModel { get; } = new();

        public NewsArticleView()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Article == null)
                throw new InvalidOperationException("Article is not initialized");

            // Using null-conditional operator for safe access
            ViewModel.HasRelatedStocks = ViewModel.Article.RelatedStocks?.Any() ?? false;
        }

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

            ViewModel.LoadArticle(articleId);
        }
    }
}