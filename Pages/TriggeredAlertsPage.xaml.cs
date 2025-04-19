namespace StockApp.Pages
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Repositories;

    public sealed partial class TriggeredAlertsPage : Page
    {
        private readonly AlertRepository _alertRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeredAlertsPage"/> class.
        /// </summary>
        public TriggeredAlertsPage()
        {
            this.InitializeComponent();
            this._alertRepository = new AlertRepository();
            this.LoadAlerts();
        }

        private void LoadAlerts()
        {
            //AlertListView.ItemsSource = _alertRepository.GetTriggeredAlerts();
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            this.LoadAlerts();
        }
    }
}
