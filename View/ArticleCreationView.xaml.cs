namespace StockNewsPage.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModel;

    public sealed partial class ArticleCreationView : Page
    {
        public ArticleCreationViewModel ViewModel { get; } = new();

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