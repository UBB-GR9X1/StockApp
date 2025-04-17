namespace StockApp.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class AdminNewsControlView : Page
    {
        public AdminNewsViewModel ViewModel { get; } = new AdminNewsViewModel();

        public AdminNewsControlView()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;

            ArticlesList.DataContext = ViewModel;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}