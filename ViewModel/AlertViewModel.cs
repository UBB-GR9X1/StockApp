using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace StockApp.ViewModel
{
    public class AlertService
    {
        private readonly List<Alert> _alerts = new List<Alert>();
        private int _nextId = 1; 

        public List<Alert> GetAll()
        {
            return _alerts;
        }

        public Alert GetById(int alertId)
        {
            return _alerts.FirstOrDefault(a => a.AlertId == alertId);
        }

        public void Create(string stockName, string name, int upperBound, int lowerBound, bool toggleOnOff)
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

        public bool Update(int alertId, string stockName, string name, int upperBound, int lowerBound, bool toggleOnOff)
        {
            var alert = GetById(alertId);
            if (alert == null) return false;

            alert.StockName = stockName;
            alert.Name = name;
            alert.UpperBound = upperBound;
            alert.LowerBound = lowerBound;
            alert.ToggleOnOff = toggleOnOff;
            return true;
        }

        public bool Delete(int alertId)
        {
            var alert = GetById(alertId);
            if (alert == null) return false;

            _alerts.Remove(alert);
            return true;
        }
    }
}
