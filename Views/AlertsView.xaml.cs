namespace StockApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class AlertsView : Page
    {
        public AlertsView()
        {
            this.InitializeComponent();
            this.DataContext = this.AlertViewModel;
        }

        public AlertViewModel AlertViewModel { get; } = new ();
    }
}
