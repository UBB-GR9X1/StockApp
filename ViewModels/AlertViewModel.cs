namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Services;

    public class AlertViewModel
    {
        private readonly IAlertService AlertService;
        private readonly IDialogService DialogService;

        public ObservableCollection<Alert> Alerts { get; } = [];

        public ICommand CreateAlertCommand { get; }

        public ICommand SaveAlertsCommand { get; }

        public ICommand DeleteAlertCommand { get; }

        public ICommand CloseAppCommand { get; }

        public AlertViewModel(IAlertService alertService, IDialogService dialogService)
        {
            this.AlertService = alertService ?? throw new ArgumentNullException(nameof(alertService));
            this.DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Initialize commands exactly as before
            this.CreateAlertCommand = new RelayCommand(async _ => await this.CreateAlert());
            this.SaveAlertsCommand = new RelayCommand(async _ => await this.SaveAlerts());
            this.DeleteAlertCommand = new RelayCommand(async p => await this.DeleteAlert(p));
            this.CloseAppCommand = new RelayCommand(_ => Environment.Exit(0));

            // Load initial alerts
            this.LoadAlerts();
        }

        public AlertViewModel()
            : this(new AlertService(), new DialogService())
        {
        }

        private static void ValidateAlert(Alert alert)
        {
            if (alert.LowerBound > alert.UpperBound)
            {
                throw new Exception("Lower bound cannot be greater than upper bound.");
            }
        }

        private async Task CreateAlert()
        {
            try
            {
                Alert NewAlert = this.AlertService.CreateAlert("Tesla", "New Alert", 100, 0, true);
                this.Alerts.Add(NewAlert);
                await this.DialogService.ShowMessageAsync("Success", "Alert created successfully!");
            }
            catch (Exception exception)
            {
                await this.DialogService.ShowMessageAsync("Error", exception.Message);
            }
        }

        private async Task SaveAlerts()
        {
            try
            {
                foreach (Alert alert in this.Alerts)
                {
                    ValidateAlert(alert);
                    this.AlertService.UpdateAlert(
                        alert.AlertId,
                        alert.StockName,
                        alert.Name,
                        alert.UpperBound,
                        alert.LowerBound,
                        alert.ToggleOnOff
                    );
                }

                await this.DialogService.ShowMessageAsync("Success", "All alerts saved successfully!");
            }
            catch (Exception exception)
            {
                await this.DialogService.ShowMessageAsync("Error", exception.Message);
            }
        }

        private async Task DeleteAlert(object parameter)
        {
            if (parameter is Alert alertToDelete)
            {
                this.AlertService.RemoveAlert(alertToDelete.AlertId);
                this.Alerts.Remove(alertToDelete);
                await this.DialogService.ShowMessageAsync("Success", "Alert deleted successfully!");
            }
            else
            {
                await this.DialogService.ShowMessageAsync("Error", "Please select an alert to delete.");
            }
        }

        private void LoadAlerts()
        {
            this.Alerts.Clear();
            foreach (Alert alert in this.AlertService.GetAllAlerts())
            {
                this.Alerts.Add(alert);
            }
        }
    }
}
