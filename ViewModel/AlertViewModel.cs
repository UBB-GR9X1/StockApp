namespace StockApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using StockApp.Models;

    public class AlertService
    {
        private readonly List<Alert> _alerts = new List<Alert>();
        private int _nextId = 1; 

        public List<Alert> GetAllAlerts()
        {
            return _alerts;
        }

        public Alert GetAlertById(int alertId)
        {
            return _alerts.FirstOrDefault(a => a.AlertId == alertId);
        }

        public void CreateAlert(string stockName, string name, int upperBound, int lowerBound, bool toggleOnOff)
        {
            var newAlert = new Alert
            {
                AlertId = _nextId++, 
                StockName = stockName,
                Name = name,
                UpperBound = upperBound,
                LowerBound = lowerBound,
                ToggleOnOff = toggleOnOff
            };
            _alerts.Add(newAlert);
        }

        public bool UpdateAlert(int alertId, string stockName, string name, int upperBound, int lowerBound, bool toggleOnOff)
        {
            var alert = GetAlertById(alertId);
            if (alert == null) return false;

            alert.StockName = stockName;
            alert.Name = name;
            alert.UpperBound = upperBound;
            alert.LowerBound = lowerBound;
            alert.ToggleOnOff = toggleOnOff;
            return true;
        }

        public bool DeleteAlert(int alertId)
        {
            var alert = GetAlertById(alertId);
            if (alert == null) return false;

            _alerts.Remove(alert);
            return true;
        }
    }
}
