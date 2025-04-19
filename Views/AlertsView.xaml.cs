namespace StockApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class AlertsView : Page
    {
        public AlertsView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
        }

        public AlertViewModel ViewModel { get; } = new ();
    }
}
