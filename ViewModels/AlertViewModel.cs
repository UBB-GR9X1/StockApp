﻿namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
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
        private string newAlertName = string.Empty;
        private decimal? alertUpperBound;
        private decimal? alertLowerBound;
        private bool alertValid = false;

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
            this.CreateAlertCommand = new RelayCommand(async _ => await this.CreateAlert(), this.IsAlertValid);
            this.SaveAlertsCommand = new RelayCommand(async _ => await this.SaveAlerts());
            this.DeleteAlertCommand = new RelayCommand(async p => await this.DeleteAlert(p));
            this.CloseAppCommand = new RelayCommand(_ => NavigationService.Instance.GoBack());

            // Load existing alerts into the collection
            this.LoadAlerts();
        }

        /// <summary>
        /// Event triggered when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

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
        /// Gets the collection of alerts displayed in the UI.
        /// </summary>
        public ObservableCollection<Alert> Alerts { get; } = [];

        /// <summary>
        /// Gets or sets the selected stock for this viewmodel.
        /// </summary>
        public string SelectedStockName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user-defined name for the new alert. Bound to a TextBox in the UI.
        /// </summary>
        public string NewAlertName
        {
            get => this.newAlertName;
            set
            {
                if (this.newAlertName != value)
                {
                    this.newAlertName = value;
                    this.OnPropertyChanged();
                }

                this.AlertValid = this.IsAlertValid();
            }
        }

        /// <summary>
        /// Gets or sets the upper price boundary for the new alert. Bound to a TextBox in the UI.
        /// </summary>
        public string NewAlertUpperBound
        {
            get => this.alertUpperBound.ToString() ?? "0";
            set
            {
                if (this.alertUpperBound.ToString() != value)
                {
                    if (decimal.TryParse(value, out var parseResult))
                    {
                        this.alertUpperBound = parseResult;
                        this.OnPropertyChanged();
                    }
                    else
                    {
                        this.alertUpperBound = null;
                    }

                    this.AlertValid = this.IsAlertValid();
                }
            }
        }

        /// <summary>
        /// Gets or sets the lower price boundary for the new alert. Bound to a TextBox in the UI.
        /// </summary>
        public string NewAlertLowerBound
        {
            get => this.alertLowerBound.ToString() ?? "0";
            set
            {
                if (this.alertLowerBound.ToString() != value)
                {
                    if (decimal.TryParse(value, out var parseResult))
                    {
                        this.alertLowerBound = parseResult;
                        this.OnPropertyChanged();
                    }
                    else
                    {
                        this.alertLowerBound = null;
                    }

                    this.AlertValid = this.IsAlertValid();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the alert to be inserted is valid.
        /// </summary>
        public bool AlertValid
        {
            get => this.alertValid;
            set
            {
                if (this.alertValid != value)
                {
                    this.alertValid = value;
                    this.OnPropertyChanged();
                    (this.CreateAlertCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private bool IsAlertValid(object? obj = null)
        {
            return !string.IsNullOrWhiteSpace(this.NewAlertName) &&
                   this.alertUpperBound != null &&
                   this.alertLowerBound != null &&
                   this.alertUpperBound > this.alertLowerBound;
        }

        /// <summary>
        /// Notifies the UI of property changes.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Creates a new alert with example data and shows a success or error dialog.
        /// </summary>
        private async Task CreateAlert()
        {
            try
            {
                decimal upperBound = this.alertUpperBound ?? throw new ArgumentNullException(nameof(this.alertUpperBound));
                decimal lowerBound = this.alertLowerBound ?? throw new ArgumentNullException(nameof(this.alertLowerBound));

                // Create a new alert via the service and add it to the collection
                Alert newAlert = await this.alertService.CreateAlertAsync(
                    stockName: this.SelectedStockName,
                    name: this.NewAlertName,
                    upperBound: upperBound,
                    lowerBound: lowerBound,
                    toggleOnOff: true);
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
                    if (alert.LowerBound >= alert.UpperBound)
                    {
                        throw new ArgumentException("Lower bound must be less than upper bound.");
                    }

                    if (string.IsNullOrWhiteSpace(alert.Name))
                    {
                        throw new ArgumentException("Alert name cannot be empty.");
                    }

                    await this.alertService.UpdateAlertAsync(
                        alert.AlertId,
                        alert.StockName,
                        alert.Name,
                        alert.UpperBound,
                        alert.LowerBound,
                        alert.ToggleOnOff);
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
                await this.alertService.RemoveAlertAsync(alertToDelete.AlertId);
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
        private async void LoadAlerts()
        {
            this.Alerts.Clear();
            foreach (Alert alert in await this.alertService.GetAllAlertsAsync())
            {
                this.Alerts.Add(alert);
            }
        }
    }
}
