namespace StockApp.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class AdminNewsControlView : Page
    {
        public AdminNewsControlView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
            this.Loaded += this.OnLoaded;
        }

        public AdminNewsViewModel ViewModel { get; } = new ();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Initialize();
        }
    }
}
