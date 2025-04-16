using Microsoft.UI.Xaml.Controls;
using StockNewsPage.ViewModels;

namespace StockNewsPage.Views
{
    public sealed partial class ArticleCreationView : Page
    {
        public ViewModels.Model ViewModel { get; } = new ViewModels.Model();

        public ArticleCreationView()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}