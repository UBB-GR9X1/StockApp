using Microsoft.UI.Xaml.Controls;
using StockNewsPage.ViewModels;

namespace StockNewsPage.Views
{
    public sealed partial class ArticleCreationView : Page
    {
        public ArticleCreationViewModel ViewModel { get; } = new ArticleCreationViewModel();

        public ArticleCreationView()
        {
            this.InitializeComponent();
            this.Loaded += ArticleCreationView_Loaded;
        }

        private void ArticleCreationView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}