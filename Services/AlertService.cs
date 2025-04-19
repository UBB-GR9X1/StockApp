namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;
    using StockApp.Repositories;

    public class AlertService : IAlertService
    {
        /// <summary>
        /// The repository for managing alerts.
        /// </summary>
        private readonly AlertRepository repository = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertService"/> class.
        /// </summary>
        /// <returns></returns>
        public List<Alert> GetAllAlerts() => this.repository.Alerts;

        /// <summary>
        /// Gets all alerts that are currently toggled on.
        /// </summary>
        /// <returns></returns>
        public List<Alert> GetAllAlertsOn() => this.repository.Alerts.FindAll(a => a.ToggleOnOff);

        /// <summary>
        /// Gets an alert by its unique identifier.
        /// </summary>
        /// <param name="alertId"></param>
        /// <returns></returns>
        public Alert? GetAlertById(int alertId) => this.repository.Alerts.Find(a => a.AlertId == alertId);

        /// <summary>
        /// Creates a new alert with the specified parameters.
        /// </summary>
        /// <param name="stockName"></param>
        /// <param name="name"></param>
        /// <param name="upperBound"></param>
        /// <param name="lowerBound"></param>
        /// <param name="toggleOnOff"></param>
        /// <returns></returns>
        public Alert CreateAlert(string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff) =>
            this.repository.AddAlert(stockName, name, upperBound, lowerBound, toggleOnOff);

        /// <summary>
        /// Updates an existing alert with the specified parameters.
        /// </summary>
        /// <param name="alertId"></param>
        /// <param name="stockName"></param>
        /// <param name="name"></param>
        /// <param name="upperBound"></param>
        /// <param name="lowerBound"></param>
        /// <param name="toggleOnOff"></param>
        public void UpdateAlert(int alertId, string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff) =>
            this.repository.UpdateAlert(alertId, stockName, name, upperBound, lowerBound, toggleOnOff);

        /// <summary>
        /// Updates an existing alert with the specified alert object.
        /// </summary>
        /// <param name="alert"></param>
        public void UpdateAlert(Alert alert) => this.repository.UpdateAlert(
            alert.AlertId,
            alert.StockName,
            alert.Name,
            alert.UpperBound,
            alert.LowerBound,
            alert.ToggleOnOff);

        /// <summary>
        /// Removes an alert by its unique identifier.
        /// </summary>
        /// <param name="alertId"></param>
        public void RemoveAlert(int alertId) => this.repository.DeleteAlert(alertId);
    }
}