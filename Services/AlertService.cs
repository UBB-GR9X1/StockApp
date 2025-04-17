namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    public class AlertService : IAlertService
    {
        private readonly AlertRepository repository = new ();

        public IReadOnlyList<IAlert> GetAllAlerts() => repository.GetAllAlerts();

        public IReadOnlyList<IAlert> GetAllAlertsOn()
        => GetAllAlerts()
           .Where(a => a.ToggleOnOff)
           .ToList();

        public void CreateAlert(IAlert alert) => repository.AddAlert(alert);

        public void RemoveAlert(int alertId) => repository.DeleteAlert(alertId);

        public void UpdateAlert(IAlert alert) => repository.UpdateAlert(alert);
    }
}