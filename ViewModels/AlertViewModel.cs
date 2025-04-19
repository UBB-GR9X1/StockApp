namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Services;

    /// <summary>
    /// ViewModel responsible for managing alerts: creating, saving, deleting, and loading alert entries.
    /// </summary>
    public class AlertViewModel
    {
        private readonly IAlertService alertService;
        private readonly IDialogService dialogService;

        /// <summary>
        /// Gets the collection of alerts displayed in the UI.
        /// </summary>
        public ObservableCollection<Alert> Alerts { get; } = new ObservableCollection<Alert>();

        /// <summary>
        /// Gets the command to create a new alert.
        /// </summary>
        public ICommand CreateAlertCommand { get; }

        /// <summary>
        /// Gets the command to save all current alerts.
        /// </summary>
        public ICommand SaveAlertsCommand { get; }

        /// <summary>
        /// Gets the command to delete a selected alert.
        /// </summary>
        public ICommand DeleteAlertCommand { get; }

        /// <summary>
        /// Gets the command to close the application.
        /// </summary>
        public ICommand CloseAppCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertViewModel"/> class with specified services.
        /// </summary>
        /// <param name="alertService">Service for CRUD operations on alerts.</param>
        /// <param name="dialogService">Service for showing dialogs and messages to the user.</param>
        public AlertViewModel(IAlertService alertService, IDialogService dialogService)
        {
            this.alertService = alertService ?? throw new ArgumentNullException(nameof(alertService));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Initialize commands
            this.CreateAlertCommand = new RelayCommand(async _ => await this.CreateAlert());
            this.SaveAlertsCommand = new RelayCommand(async _ => await this.SaveAlerts());
            this.DeleteAlertCommand = new RelayCommand(async p => await this.DeleteAlert(p));
            this.CloseAppCommand = new RelayCommand(_ => Environment.Exit(0));

            // Load existing alerts into the collection
            this.LoadAlerts();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertViewModel"/> class using default implementations.
        /// </summary>
        public AlertViewModel()
            : this(new AlertService(), new DialogService())
        {
        }

        /// <summary>
        /// Validates that the alert’s lower bound does not exceed its upper bound.
        /// </summary>
        /// <param name="alert">The alert to validate.</param>
        /// <exception cref="Exception">Thrown when the lower bound is greater than the upper bound.</exception>
        private static void ValidateAlert(Alert alert)
        {
            if (alert.LowerBound > alert.UpperBound)
            {
                throw new Exception("Lower bound cannot be greater than upper bound.");
            }
        }

        /// <summary>
        /// Creates a new alert with example data and shows a success or error dialog.
        /// </summary>
        private async Task CreateAlert()
        {
            try
            {
                // Create a new alert via the service and add it to the collection
                Alert newAlert = this.alertService.CreateAlert("Tesla", "New Alert", 100, 0, true);
                this.Alerts.Add(newAlert);
                await this.dialogService.ShowMessageAsync("Success", "Alert created successfully!");
            }
            catch (Exception exception)
            {
                await this.dialogService.ShowMessageAsync("Error", exception.Message);
            }
        }

        /// <summary>
        /// Saves all current alerts by validating and updating each via the service.
        /// </summary>
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
            catch (Exception exception)
            {
                await this.dialogService.ShowMessageAsync("Error", exception.Message);
            }
        }

        /// <summary>
        /// Deletes the specified alert after confirmation and shows a result dialog.
        /// </summary>
        /// <param name="parameter">The alert object to delete (or another parameter).</param>
        private async Task DeleteAlert(object parameter)
        {
            if (parameter is Alert alertToDelete)
            {
                this.alertService.RemoveAlert(alertToDelete.AlertId);
                this.Alerts.Remove(alertToDelete);
                await this.dialogService.ShowMessageAsync("Success", "Alert deleted successfully!");
            }
            else
            {
                await this.dialogService.ShowMessageAsync("Error", "Please select an alert to delete.");
            }
        }

        /// <summary>
        /// Loads all alerts from the service into the <see cref="Alerts"/> collection.
        /// </summary>
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
