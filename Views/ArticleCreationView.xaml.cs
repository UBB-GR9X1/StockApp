namespace StockApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class ArticleCreationView : Page
    {
        public ArticleCreationView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
            this.Loaded += this.OnLoaded;
        }

        public ArticleCreationViewModel ViewModel { get; } = new ();

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.ViewModel.Initialize();
        }
    }
}