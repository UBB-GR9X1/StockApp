﻿namespace StockApp.Models
{
    using System;

    /// <summary>
    /// Defines criteria for filtering stock transactions.
    /// </summary>
    public class TransactionFilterCriteria
    {
        /// <summary>
        /// Gets or sets the name of the stock to filter by.
        /// </summary>
        public string? StockName { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction to filter by ("BUY" or "SELL").
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the minimum total value for matching transactions.
        /// </summary>
        public int? MinTotalValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum total value for matching transactions.
        /// </summary>
        public int? MaxTotalValue { get; set; }

        /// <summary>
        /// Gets or sets the earliest transaction date to include.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the latest transaction date to include.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Validates the filter criteria, throwing if any range or value is invalid.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if <see cref="Type"/> is not "BUY" or "SELL",
        /// or if <see cref="MinTotalValue"/> exceeds <see cref="MaxTotalValue"/>,
        /// or if <see cref="StartDate"/> is after <see cref="EndDate"/>.
        /// </exception>
        public void Validate()
        {
            // Ensure that if a Type is specified, it's either "BUY" or "SELL"
            if (!string.IsNullOrEmpty(this.Type)
                && this.Type is not ("BUY" or "SELL"))
            {
                throw new ArgumentException(
                    "Type must be \"BUY\" or \"SELL\".");
            }

            // Ensure that the minimum total value does not exceed the maximum
            if (this.MinTotalValue.HasValue
                && this.MaxTotalValue.HasValue
                && this.MinTotalValue > this.MaxTotalValue)
            {
                throw new ArgumentException(
                    "Min cannot exceed Max total value.");
            }

            // Ensure that the start date is not later than the end date
            if (this.StartDate.HasValue
                && this.EndDate.HasValue
                && this.StartDate > this.EndDate)
            {
                throw new ArgumentException(
                    "StartDate cannot be after EndDate.");
            }
        }
    }
}
