using System.Collections.Generic;
using Models;
using StockApp.Repository;

namespace StockApp.Service
{
    public class AlertService
    {
        private readonly AlertRepository _repository = new AlertRepository();

        public List<Alert> GetAllAlerts() => _repository.GetAllAlerts();

        public List<Alert> GetAllAlertsOn() => _repository.GetAllAlerts().FindAll(a => a.ToggleOnOff);

        public void CreateAlert(Alert alert) => _repository.AddAlert(alert);

        public void RemoveAlert(int alertId) => _repository.DeleteAlert(alertId);

        public void UpdateAlert(Alert alert) => _repository.UpdateAlert(alert);




    }
}