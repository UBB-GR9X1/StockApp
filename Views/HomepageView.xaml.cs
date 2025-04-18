namespace StockApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class HomepageView : Page
    {
        public HomepageView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
        }

        public HomepageViewModel ViewModel { get; } = new ();
    }
}