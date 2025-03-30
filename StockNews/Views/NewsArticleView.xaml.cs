using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using StockNewsPage.ViewModels;

namespace StockNewsPage.Views
{
    public sealed partial class NewsArticleView : Page
    {
        public NewsDetailViewModel ViewModel { get; } = new NewsDetailViewModel();

        public NewsArticleView()
        {
            this.InitializeComponent();
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