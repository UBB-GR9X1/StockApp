namespace StockApp.Service
{
    using System.Collections.Generic;
    using StockApp.Models;
    using StockApp.Repository;

    public class AlertService
    {
        private readonly AlertRepository repository = new ();

        public List<Alert> GetAllAlerts() => repository.GetAllAlerts();

        public List<Alert> GetAllAlertsOn() => repository.GetAllAlerts().FindAll(a => a.ToggleOnOff);

        public void CreateAlert(Alert alert) => repository.AddAlert(alert);

        public void RemoveAlert(int alertId) => repository.DeleteAlert(alertId);

        public void UpdateAlert(Alert alert) => repository.UpdateAlert(alert);
    }
}