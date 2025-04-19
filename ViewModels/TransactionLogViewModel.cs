namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Models;
    using StockApp.Repositories;
    using StockApp.Services;

    /// <summary>
    /// ViewModel for displaying and exporting the transaction log with filtering and sorting capabilities.
    /// </summary>
    public class TransactionLogViewModel : INotifyPropertyChanged
    {
        private readonly ITransactionLogService service;

        private string stockNameFilter;
        private string selectedTransactionType;
        private string selectedSortBy;
        private string selectedSortOrder;
        private string selectedExportFormat;
        private string minTotalValue;
        private string maxTotalValue;
        private DateTime? startDate;
        private DateTime? endDate;

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Event requested to show a message box with a given title and content.
        /// </summary>
        public event Action<string, string> ShowMessageBoxRequested;

        /// <summary>
        /// Gets the collection of transactions displayed in the log.
        /// </summary>
        public ObservableCollection<TransactionLogTransaction> Transactions { get; set; } = new ObservableCollection<TransactionLogTransaction>();

        /// <summary>
        /// Gets or sets the filter text for the stock name.
        /// </summary>
        public string StockNameFilter
        {
            get => this.stockNameFilter;
            set { this.stockNameFilter = value; this.OnPropertyChanged(nameof(this.StockNameFilter)); }
        }

        /// <summary>
        /// Gets or sets the selected transaction type for filtering.
        /// </summary>
        public string SelectedTransactionType
        {
            get => this.selectedTransactionType;
            set
            {
                this.selectedTransactionType = value;
                this.OnPropertyChanged(nameof(this.SelectedTransactionType));
                this.LoadTransactions(); // Reload transactions when the selected type changes
            }
        }

        /// <summary>
        /// Gets or sets the criteria by which to sort the transactions.
        /// </summary>
        public string SelectedSortBy
        {
            get => this.selectedSortBy;
            set
            {
                this.selectedSortBy = value;
                this.OnPropertyChanged(nameof(this.SelectedSortBy));
                this.LoadTransactions(); // Reload transactions when the sorting criteria change
            }
        }

        /// <summary>
        /// Gets or sets the sort order (ascending/descending).
        /// </summary>
        public string SelectedSortOrder
        {
            get => this.selectedSortOrder;
            set
            {
                this.selectedSortOrder = value;
                this.OnPropertyChanged(nameof(this.SelectedSortOrder));
                this.LoadTransactions(); // Reload transactions when the sort order changes
            }
        }

        /// <summary>
        /// Gets or sets the export format (e.g., CSV, JSON).
        /// </summary>
        public string SelectedExportFormat
        {
            get => this.selectedExportFormat;
            set
            {
                this.selectedExportFormat = value;
                this.OnPropertyChanged(nameof(this.SelectedExportFormat));
            }
        }

        /// <summary>
        /// Gets or sets the minimum total value filter.
        /// </summary>
        public string MinTotalValue
        {
            get => this.minTotalValue;
            set
            {
                if (this.ValidateNumericValue(value))
                {
                    this.minTotalValue = value;
                    this.OnPropertyChanged(nameof(this.MinTotalValue));
                    this.LoadTransactions();
                }
                else
                {
                    this.ShowMessageBox("Invalid Input", "Min Total Value must be a valid integer.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum total value filter.
        /// </summary>
        public string MaxTotalValue
        {
            get => this.maxTotalValue;
            set
            {
                if (this.ValidateNumericValue(value))
                {
                    this.maxTotalValue = value;
                    this.OnPropertyChanged(nameof(this.MaxTotalValue));
                    this.LoadTransactions();
                }
                else
                {
                    this.ShowMessageBox("Invalid Input", "Max Total Value must be a valid integer.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the start date of the transaction date range filter.
        /// </summary>
        public DateTime? StartDate
        {
            get => this.startDate;
            set { this.startDate = value; this.OnPropertyChanged(nameof(this.StartDate)); this.LoadTransactions(); }
        }

        /// <summary>
        /// Gets or sets the end date of the transaction date range filter.
        /// </summary>
        public DateTime? EndDate
        {
            get => this.endDate;
            set { this.endDate = value; this.OnPropertyChanged(nameof(this.EndDate)); this.LoadTransactions(); }
        }

        /// <summary>
        /// Gets the command to perform a search with current filters.
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// Gets the command to export the current transaction list.
        /// </summary>
        public ICommand ExportCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogViewModel"/> class with the specified service.
        /// </summary>
        /// <param name="service">Service to retrieve and export transaction data.</param>
        public TransactionLogViewModel(ITransactionLogService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));

            // Initialize ComboBoxItems for options if they are null
            this.selectedTransactionType = "ALL";
            this.selectedSortBy = "Date";
            this.selectedSortOrder = "ASC";
            this.selectedExportFormat = "CSV";

            // Set up commands
            this.SearchCommand = new Commands.Command(this.Search);
            this.ExportCommand = new Commands.Command(async () => await this.Export());

            this.LoadTransactions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogViewModel"/> class with default repository and service.
        /// </summary>
        public TransactionLogViewModel()
            : this(new TransactionLogService(new TransactionRepository()))
        { }

        /// <summary>
        /// Reloads the transaction list based on current filters and sorting.
        /// </summary>
        private void Search()
        {
            this.LoadTransactions();
        }

        /// <summary>
        /// Exports the transactions to a file in the selected format.
        /// </summary>
        /// <returns>A task representing the asynchronous export operation.</returns>
        private async Task Export()
        {
            string format = this.selectedExportFormat.ToString();
            string fileName = "transactions";

            // Inline: determine user's Documents path
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = Path.Combine(documentsPath, $"{fileName}.{format.ToLower()}");

            // Export the transactions
            this.service.ExportTransactions(this.Transactions.ToList(), fullPath, format);

            // TODO: notify user of successful export
            // ShowMessageBox("Export Successful", $"File saved: {fullPath}");
        }

        /// <summary>
        /// Raises a request to show a message box with the specified title and content.
        /// </summary>
        /// <param name="title">Title of the message box.</param>
        /// <param name="content">Content text of the message box.</param>
        public void ShowMessageBox(string title, string content)
        {
            this.ShowMessageBoxRequested?.Invoke(title, content);
        }

        /// <summary>
        /// Validates that the provided string represents a valid integer value.
        /// </summary>
        /// <param name="value">Input string to validate.</param>
        /// <returns><c>true</c> if valid; otherwise, <c>false</c>.</returns>
        private bool ValidateNumericValue(string value)
        {
            // FIXME: consider allowing decimal values for filters
            return int.TryParse(value, out _);
        }

        /// <summary>
        /// Validates that the minimum value is less than the maximum value.
        /// </summary>
        /// <param name="minTotalValue">Minimum total value.</param>
        /// <param name="maxTotalValue">Maximum total value.</param>
        /// <returns><c>true</c> if valid or not applicable; otherwise, <c>false</c>.</returns>
        private bool ValidateTotalValues(string minTotalValue, string maxTotalValue)
        {
            if (int.TryParse(minTotalValue, out int min) && int.TryParse(maxTotalValue, out int max))
            {
                return min < max;
            }
            return true;
        }

        /// <summary>
        /// Validates that the start date is before the end date.
        /// </summary>
        /// <param name="startDate">Start date value.</param>
        /// <param name="endDate">End date value.</param>
        /// <returns><c>true</c> if valid or not applicable; otherwise, <c>false</c>.</returns>
        private bool ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return startDate.Value < endDate.Value;
            }
            return true;
        }

        /// <summary>
        /// Loads and filters transactions from the service and applies sorting.
        /// </summary>
        public void LoadTransactions()
        {
            if (this.service == null)
            {
                throw new InvalidOperationException("Transaction service is not initialized");
            }

            // Add null checks here for all ComboBoxItem properties to prevent null reference
            string transactionType = this.selectedTransactionType?.ToString() ?? "ALL";
            string sortBy = this.selectedSortBy?.ToString() ?? "Date";
            string sortOrder = this.selectedSortOrder?.ToString() ?? "ASC";

            // Validate MinTotalValue < MaxTotalValue
            if (!this.ValidateTotalValues(this.minTotalValue, this.maxTotalValue))
            {
                this.ShowMessageBox("Invalid Total Values", "Min Total Value must be less than Max Total Value.");
                return;
            }

            // Validate StartDate < EndDate
            if (!this.ValidateDateRange(this.startDate, this.endDate))
            {
                this.ShowMessageBox("Invalid Date Range", "Start Date must be earlier than End Date.");
                return;
            }

            DateTime startDate = this.startDate ?? DateTime.Now.AddYears(-10);
            DateTime endDate = this.endDate ?? DateTime.Now;

            this.Transactions.Clear();

            var filterCriteria = new TransactionFilterCriteria
            {
                StockName = this.stockNameFilter,
                Type = transactionType == "ALL" ? null : transactionType,
                MinTotalValue = string.IsNullOrEmpty(this.minTotalValue) ? null : Convert.ToInt32(this.minTotalValue),
                MaxTotalValue = string.IsNullOrEmpty(this.maxTotalValue) ? null : Convert.ToInt32(this.maxTotalValue),
                StartDate = startDate,
                EndDate = endDate,
            };

            filterCriteria.Validate(); // Inline: ensure criteria consistency

            var transactions = this.service.GetFilteredTransactions(filterCriteria)
                ?? throw new InvalidOperationException("Transaction service returned null");

            foreach (var transaction in transactions)
            {
                this.Transactions.Add(transaction);
            }

            this.SortTransactions(sortBy, sortOrder == "ASC");
        }

        /// <summary>
        /// Sorts the current transactions based on the specified criteria and order.
        /// </summary>
        /// <param name="sortBy">Property name to sort by.</param>
        /// <param name="isAscending">Whether to sort ascending (<c>true</c>) or descending (<c>false</c>).</param>
        private void SortTransactions(string sortBy, bool isAscending)
        {
            var sortedTransactions = this.service.SortTransactions(this.Transactions.ToList(), sortBy, isAscending);

            this.Transactions.Clear();
            foreach (var transaction in sortedTransactions)
            {
                this.Transactions.Add(transaction);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property changed.</param>
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
