namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IAlertService
    {
        Alert CreateAlert(string stockName, string name, int upperBound, int lowerBound, bool toggleOnOff);
        Alert? GetAlertById(int alertId);
        List<Alert> GetAllAlerts();
        List<Alert> GetAllAlertsOn();
        void RemoveAlert(int alertId);
        void UpdateAlert(int alertId, string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff);
    }
}