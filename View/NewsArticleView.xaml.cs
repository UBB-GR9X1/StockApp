using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using StockApp.StockPage;
using StockNewsPage.ViewModels;
using System.Linq;
using StockApp.Service;

namespace StockNewsPage.Views
{
    public sealed partial class NewsArticleView : Page
    {
        public NewsDetailViewModel ViewModel { get; } = new NewsDetailViewModel();

        public NewsArticleView()
        {
            this.InitializeComponent();
            this.Loaded += NewsArticleView_Loaded;
        }

        private void NewsArticleView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Article != null)
            {
                if (ViewModel.Article.RelatedStocks != null)
                {
                    ViewModel.HasRelatedStocks = ViewModel.Article.RelatedStocks.Any();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"PAGE LOADED - RelatedStocks is NULL");
                }
            }
        }

        private void RelatedStock_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string stockName)
            {
                NavigationService.Instance.Navigate(typeof(StockPage), stockName);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string articleId)
            {
                ViewModel.LoadArticle(articleId);
            }
        }
    }
}