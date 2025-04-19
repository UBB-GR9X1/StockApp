namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    public class AlertService : IAlertService
    {
        private readonly AlertRepository repository = new();

        public List<Alert> GetAllAlerts() => repository.GetAllAlerts();

        public List<Alert> GetAllAlertsOn()
        => GetAllAlerts()
           .Where(a => a.ToggleOnOff)
           .ToList();

        public void CreateAlert(Alert alert) => repository.AddAlert(alert);

        public void RemoveAlert(int alertId) => repository.DeleteAlert(alertId);

        public void UpdateAlert(Alert alert) => repository.UpdateAlert(alert);
    }
}