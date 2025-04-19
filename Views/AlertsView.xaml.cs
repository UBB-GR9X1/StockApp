namespace StockApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class AlertsView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertsView"/> class.
        /// </summary>
        public AlertsView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
        }

        /// <summary>
        /// Gets the ViewModel for managing alerts.
        /// </summary>
        public AlertViewModel ViewModel { get; } = new ();
    }
}
