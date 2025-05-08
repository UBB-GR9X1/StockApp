﻿namespace StockApp.Models
{
    /// <summary>
    /// Represents an alert that has been triggered for a specific stock,
    /// including the stock's name and the message explaining the trigger.
    /// </summary>
    public class TriggeredAlert
    {
        /// <summary>
        /// Gets or sets the name of the stock for which the alert was triggered.
        /// </summary>
        public string StockName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message detailing why the alert was triggered.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
