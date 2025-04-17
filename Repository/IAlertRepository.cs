namespace StockApp.Repository
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IAlertRepository
    {
        IReadOnlyList<IAlert> GetAllAlerts();

        IAlert GetAlertById(int alertId);

        void AddAlert(IAlert alert);

        void UpdateAlert(IAlert alert);

        void DeleteAlert(int alertId);

        bool IsAlertTriggered(string stockName, decimal currentPrice);

        void TriggerAlert(string stockName, decimal currentPrice);

        IReadOnlyList<ITriggeredAlert> GetTriggeredAlerts();

        void ClearTriggeredAlerts();
    }
}
