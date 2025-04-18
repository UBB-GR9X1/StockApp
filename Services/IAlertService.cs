namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IAlertService
    {
        List<Alert> GetAllAlerts();

        List<Alert> GetAllAlertsOn();

        Alert CreateAlert(string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff);

        void RemoveAlert(int alertId);

        void UpdateAlert(int alertId, string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff);

        void UpdateAlert(Alert alert);
    }
}
