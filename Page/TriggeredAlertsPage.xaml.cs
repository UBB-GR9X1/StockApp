using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Repository;

namespace StockApp.Views
{
    public sealed partial class TriggeredAlertsPage : Page
    {
        private readonly AlertRepository _alertRepository;

        public TriggeredAlertsPage()
        {
            this.InitializeComponent();
            _alertRepository = new AlertRepository();
            LoadAlerts();
        }

        private void LoadAlerts()
        {
            AlertListView.ItemsSource = _alertRepository.GetTriggeredAlerts();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAlerts();
        }
    }
}
