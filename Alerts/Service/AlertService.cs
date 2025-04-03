using Alerts.Repository;
using System.Collections.Generic;
using Models;  // Changed from "Models" to "Alerts.Models" for consistency

namespace Service
{
    public class AlertService
    {
        private readonly AlertRepository _repository = new AlertRepository();

        public List<Alert> GetAllAlerts() => _repository.GetAllAlerts();  // Changed to GetAllAlerts()

        public void CreateAlert(Alert alert) => _repository.AddAlert(alert);

        public void RemoveAlert(int alertId) => _repository.DeleteAlert(alertId);

        // Optional: Add this if you need update functionality
        public void UpdateAlert(Alert alert) => _repository.UpdateAlert(alert);
    }
}