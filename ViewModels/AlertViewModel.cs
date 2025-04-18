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
        private readonly IAlertService alertService;
        private readonly IDialogService dialogService;

        public ObservableCollection<Alert> Alerts { get; } = [];

        public ICommand CreateAlertCommand { get; }

        public ICommand SaveAlertsCommand { get; }

        public ICommand DeleteAlertCommand { get; }

        public ICommand CloseAppCommand { get; }

        public AlertViewModel(IAlertService alertService, IDialogService dialogService)
        {
            this.alertService = alertService ?? throw new ArgumentNullException(nameof(alertService));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

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
                Alert newAlert = this.alertService.CreateAlert("Tesla", "New Alert", 100, 0, true);
                this.Alerts.Add(newAlert);
                await this.dialogService.ShowMessageAsync("Success", "Alert created successfully!");
            }
            catch (Exception ex)
            {
                await this.dialogService.ShowMessageAsync("Error", ex.Message);
            }
        }

        private async Task SaveAlerts()
        {
            try
            {
                foreach (Alert alert in this.Alerts)
                {
                    ValidateAlert(alert);
                    this.alertService.UpdateAlert(
                        alert.AlertId,
                        alert.StockName,
                        alert.Name,
                        alert.UpperBound,
                        alert.LowerBound,
                        alert.ToggleOnOff
                    );
                }

                await this.dialogService.ShowMessageAsync("Success", "All alerts saved successfully!");
            }
            catch (Exception ex)
            {
                await this.dialogService.ShowMessageAsync("Error", ex.Message);
            }
        }

        private async Task DeleteAlert(object parameter)
        {
            if (parameter is Alert alert)
            {
                this.alertService.RemoveAlert(alert.AlertId);
                this.Alerts.Remove(alert);
                await this.dialogService.ShowMessageAsync("Success", "Alert deleted successfully!");
            }
            else
            {
                await this.dialogService.ShowMessageAsync("Error", "Please select an alert to delete.");
            }
        }

        private void LoadAlerts()
        {
            this.Alerts.Clear();
            foreach (Alert alert in this.alertService.GetAllAlerts())
            {
                this.Alerts.Add(alert);
            }
        }
    }
}
