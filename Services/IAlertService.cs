using System.Collections.Generic;
using StockApp.Models;

namespace StockApp.Services
{
    public interface IAlertService
    {
        List<Alert> GetAllAlerts();

        List<Alert> GetAllAlertsOn();

        void CreateAlert(Alert alert);

        void RemoveAlert(int alertId);

        void UpdateAlert(Alert alert);
    }
}
