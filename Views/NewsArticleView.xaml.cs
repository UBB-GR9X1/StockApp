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
        public NewsDetailViewModel ViewModel { get; } = new();

        private string selectedArticleId;
        public NewsArticleView()
        {
            this.InitializeComponent();
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