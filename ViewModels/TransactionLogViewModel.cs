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

        private string _stockNameFilter;
        private string _selectedTransactionType;
        private string _selectedSortBy;
        private string _selectedSortOrder;
        private string _selectedExportFormat;
        private string _minTotalValue;
        private string _maxTotalValue;
        private DateTime? _startDate;
        private DateTime? _endDate;

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
            get => _stockNameFilter;
            set
            {
                _stockNameFilter = value;
                OnPropertyChanged(nameof(StockNameFilter));
            }
        }

        /// <summary>
        /// Gets or sets the selected transaction type for filtering.
        /// </summary>
        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                _selectedTransactionType = value;
                OnPropertyChanged(nameof(SelectedTransactionType));
                // Inline: reload transactions when the transaction type filter changes
                LoadTransactions();
            }
        }

        /// <summary>
        /// Gets or sets the criteria by which to sort the transactions.
        /// </summary>
        public string SelectedSortBy
        {
            get => _selectedSortBy;
            set
            {
                _selectedSortBy = value;
                OnPropertyChanged(nameof(SelectedSortBy));
                // Inline: reload transactions when sort criteria changes
                LoadTransactions();
            }
        }

        /// <summary>
        /// Gets or sets the sort order (ascending/descending).
        /// </summary>
        public string SelectedSortOrder
        {
            get => _selectedSortOrder;
            set
            {
                _selectedSortOrder = value;
                OnPropertyChanged(nameof(SelectedSortOrder));
                // Inline: reload transactions when sort order changes
                LoadTransactions();
            }
        }

        /// <summary>
        /// Gets or sets the export format (e.g., CSV, JSON).
        /// </summary>
        public string SelectedExportFormat
        {
            get => _selectedExportFormat;
            set
            {
                _selectedExportFormat = value;
                OnPropertyChanged(nameof(SelectedExportFormat));
            }
        }

        /// <summary>
        /// Gets or sets the minimum total value filter.
        /// </summary>
        public string MinTotalValue
        {
            get => _minTotalValue;
            set
            {
                if (ValidateNumericValue(value))
                {
                    _minTotalValue = value;
                    OnPropertyChanged(nameof(MinTotalValue));
                    LoadTransactions();
                }
                else
                {
                    ShowMessageBox("Invalid Input", "Min Total Value must be a valid integer.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum total value filter.
        /// </summary>
        public string MaxTotalValue
        {
            get => _maxTotalValue;
            set
            {
                if (ValidateNumericValue(value))
                {
                    _maxTotalValue = value;
                    OnPropertyChanged(nameof(MaxTotalValue));
                    LoadTransactions();
                }
                else
                {
                    ShowMessageBox("Invalid Input", "Max Total Value must be a valid integer.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the start date of the transaction date range filter.
        /// </summary>
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                // Inline: reload on date range change
                LoadTransactions();
            }
        }

        /// <summary>
        /// Gets or sets the end date of the transaction date range filter.
        /// </summary>
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                // Inline: reload on date range change
                LoadTransactions();
            }
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

            // Initialize default filter and sort options
            SelectedTransactionType = "ALL";
            SelectedSortBy = "Date";
            SelectedSortOrder = "ASC";
            SelectedExportFormat = "CSV";

            SearchCommand = new Commands.Command(Search);
            ExportCommand = new Commands.Command(async () => await Export());

            LoadTransactions();
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
            LoadTransactions();
        }

        /// <summary>
        /// Exports the transactions to a file in the selected format.
        /// </summary>
        /// <returns>A task representing the asynchronous export operation.</returns>
        private async Task Export()
        {
            string format = SelectedExportFormat;
            string fileName = "transactions";

            // Inline: determine user's Documents path
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = Path.Combine(documentsPath, $"{fileName}.{format.ToLower()}");

            // Export data using service
            service.ExportTransactions(Transactions.ToList(), fullPath, format);

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
            ShowMessageBoxRequested?.Invoke(title, content);
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
            if (service == null)
                throw new InvalidOperationException("Transaction service is not initialized.");

            // Inline: apply default filters if null
            string transactionType = SelectedTransactionType ?? "ALL";
            string sortBy = SelectedSortBy ?? "Date";
            bool ascending = SelectedSortOrder == "ASC";

            // Validate filter ranges
            if (!ValidateTotalValues(MinTotalValue, MaxTotalValue))
            {
                ShowMessageBox("Invalid Total Values", "Min Total Value must be less than Max Total Value.");
                return;
            }
            if (!ValidateDateRange(StartDate, EndDate))
            {
                ShowMessageBox("Invalid Date Range", "Start Date must be earlier than End Date.");
                return;
            }

            // Inline: set defaults for date range
            DateTime start = StartDate ?? DateTime.Now.AddYears(-10);
            DateTime end = EndDate ?? DateTime.Now;

            Transactions.Clear();

            var criteria = new TransactionFilterCriteria
            {
                StockName = StockNameFilter,
                Type = transactionType == "ALL" ? null : transactionType,
                MinTotalValue = string.IsNullOrEmpty(MinTotalValue) ? (int?)null : Convert.ToInt32(MinTotalValue),
                MaxTotalValue = string.IsNullOrEmpty(MaxTotalValue) ? (int?)null : Convert.ToInt32(MaxTotalValue),
                StartDate = start,
                EndDate = end
            };

            criteria.Validate(); // Inline: ensure criteria consistency

            var items = service.GetFilteredTransactions(criteria)
                              ?? throw new InvalidOperationException("Service returned null transactions.");

            foreach (var txn in items)
            {
                Transactions.Add(txn);
            }

            SortTransactions(sortBy, ascending);
        }

        /// <summary>
        /// Sorts the current transactions based on the specified criteria and order.
        /// </summary>
        /// <param name="sortBy">Property name to sort by.</param>
        /// <param name="ascending">Whether to sort ascending (<c>true</c>) or descending (<c>false</c>).</param>
        private void SortTransactions(string sortBy, bool ascending)
        {
            var sorted = service.SortTransactions(Transactions.ToList(), sortBy, ascending);
            Transactions.Clear();
            foreach (var txn in sorted)
            {
                Transactions.Add(txn);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property changed.</param>
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
