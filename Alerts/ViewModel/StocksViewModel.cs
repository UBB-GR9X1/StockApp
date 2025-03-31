using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Alerts.Data;
using Alerts.Models;

namespace Alerts.ViewModels
{
    public class AlertsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Alert> _alerts;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Alert> Alerts
        {
            get => _alerts;
            set
            {
                _alerts = value;
                OnPropertyChanged();
            }
        }

        public AlertsViewModel()
        {
            LoadAlerts();
        }

        private void LoadAlerts()
        {
            using (var db = new StocksAppContext())
            {
                Alerts = new ObservableCollection<Alert>(db.Alerts.ToList());
            }
        }

        public void AddAlert()
        {
            var newAlert = new Alert
            {
                Name = "New Alert",
                UpperBound = 100,
                LowerBound = 0,
                ToggleOnOff = false
            };
            Alerts.Add(newAlert);
        }

        public async Task DeleteAlert(Alert alert)
        {
            if (alert != null)
            {
                Alerts.Remove(alert);
                using (var db = new StocksAppContext())
                {
                    if (alert.AlertId != 0)
                    {
                        var alertToDelete = db.Alerts.FirstOrDefault(a => a.AlertId == alert.AlertId);
                        if (alertToDelete != null)
                        {
                            db.Alerts.Remove(alertToDelete);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public async Task SaveAlerts()
        {
            using (var db = new StocksAppContext())
            {
                foreach (var alert in Alerts)
                {
                    var existingAlert = db.Alerts.FirstOrDefault(a => a.AlertId == alert.AlertId);
                    if (existingAlert == null)
                    {
                        db.Alerts.Add(alert);
                    }
                    else
                    {
                        existingAlert.Name = alert.Name;
                        existingAlert.UpperBound = alert.UpperBound;
                        existingAlert.LowerBound = alert.LowerBound;
                        existingAlert.ToggleOnOff = alert.ToggleOnOff;
                    }
                }
                db.SaveChanges();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}