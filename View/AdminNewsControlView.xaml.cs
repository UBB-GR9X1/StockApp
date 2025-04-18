namespace StockNewsPage.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModel;

    public sealed partial class AdminNewsControlView : Page
    {
        public AdminNewsViewModel ViewModel { get; } = new ();

        public AdminNewsControlView()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;

            this.ArticlesList.DataContext = this.ViewModel;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Initialize();
        }
    }
}
