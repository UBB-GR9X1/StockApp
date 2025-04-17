using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Service
{
    public interface IAlertService
    {
        IReadOnlyList<IAlert> GetAllAlerts();

        IReadOnlyList<IAlert> GetAllAlertsOn();

        void CreateAlert(IAlert alert);

        void RemoveAlert(int alertId);

        void UpdateAlert(IAlert alert);
    }
}
