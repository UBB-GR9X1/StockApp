namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
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
        private string selectedTransactionType = "ALL";
        private string selectedSortBy = "Date";
        private string selectedSortOrder = "ASC";
        private string selectedExportFormat = "CSV";
        private string minTotalValue;
        private string maxTotalValue;
        private DateTime startDate = DateTime.UnixEpoch;
        private DateTime endDate = DateTime.Now;

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
        public ObservableCollection<TransactionLogTransaction> Transactions { get; set; } = [];

        /// <summary>
        /// Gets or sets the filter text for the stock name.
        /// </summary>
        public string StockNameFilter
        {
            get => this.stockNameFilter;
            set
            {
                this.stockNameFilter = value;
                this.OnPropertyChanged(nameof(this.StockNameFilter));
            }
        }

        /// <summary>
        /// Gets or sets the selected transaction type for filtering.
        /// </summary>
        public ComboBoxItem SelectedTransactionType
        {
            get => new ComboBoxItem { Content = this.selectedTransactionType };
            set
            {
                this.selectedTransactionType = value.Content.ToString() ?? string.Empty;
                this.OnPropertyChanged(nameof(this.SelectedTransactionType));
                this.LoadTransactions(); // Reload transactions when the selected type changes
            }
        }

        /// <summary>
        /// Gets or sets the criteria by which to sort the transactions.
        /// </summary>
        public ComboBoxItem SelectedSortBy
        {
            get => new ComboBoxItem { Content = this.selectedSortBy };
            set
            {
                this.selectedSortBy = value.Content.ToString() ?? string.Empty;
                this.OnPropertyChanged(nameof(this.SelectedSortBy));
                this.LoadTransactions(); // Reload transactions when the sorting criteria change
            }
        }

        /// <summary>
        /// Gets or sets the sort order (ascending/descending).
        /// </summary>
        public ComboBoxItem SelectedSortOrder
        {
            get => new ComboBoxItem { Content = this.selectedSortOrder };
            set
            {
                this.selectedSortOrder = value.Content.ToString() ?? string.Empty;
                this.OnPropertyChanged(nameof(this.SelectedSortOrder));
                this.LoadTransactions(); // Reload transactions when the sort order changes
            }
        }

        /// <summary>
        /// Gets or sets the export format (e.g., CSV, JSON).
        /// </summary>
        public ComboBoxItem SelectedExportFormat
        {
            get => new ComboBoxItem { Content = this.selectedExportFormat };
            set
            {
                this.selectedExportFormat = value.Content.ToString() ?? string.Empty;
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
                if (ValidateNumericValue(value))
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
                if (ValidateNumericValue(value))
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
        public DateTime StartDate
        {
            get => this.startDate;
            set
            {
                this.startDate = value;
                this.OnPropertyChanged(nameof(this.StartDate));
                this.LoadTransactions();
            }
        }

        /// <summary>
        /// Gets the start date of the transaction date range filter as a DateTimeOffset.
        /// </summary>
        public DateTimeOffset StartDateOffset
        {
            get => new(this.startDate);
            set
            {
                this.startDate = value.DateTime;
                this.OnPropertyChanged(nameof(this.StartDateOffset));
            }
        }

        /// <summary>
        /// Gets or sets the end date of the transaction date range filter.
        /// </summary>
        public DateTime EndDate
        {
            get => this.endDate;
            set
            {
                this.endDate = value;
                this.OnPropertyChanged(nameof(this.EndDate));
                this.LoadTransactions();
            }
        }

        /// <summary>
        /// Gets the end date of the transaction date range filter as a DateTimeOffset.
        /// </summary>
        public DateTimeOffset EndDateOffset
        {
            get => new(this.endDate);
            set
            {
                this.endDate = value.DateTime;
                this.OnPropertyChanged(nameof(this.EndDateOffset));
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
        /// Initializes a new instance of the <see cref="TransactionLogViewModel"/> class with the specified homepageService.
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
            this.ExportCommand = new Commands.Command(async () => this.Export());

            this.LoadTransactions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogViewModel"/> class with default repository and homepageService.
        /// </summary>
        public TransactionLogViewModel()
            : this(new TransactionLogService(new TransactionRepository()))
        {
        }

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
        private async void Export()
        {
            string format = this.selectedExportFormat.ToString();
            string fileName = "transactions";

            string fileExtension = format switch
            {
                "CSV" => ".csv",
                "JSON" => ".json",
                "HTML" => ".html",
                _ => throw new InvalidOperationException($"Unsupported export format: {format}"),
            };

            Windows.Storage.Pickers.FileSavePicker saveFileDialog = new()
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                FileTypeChoices =
                {
                    { format, new List<string> { fileExtension } },
                },
                SuggestedFileName = fileName,
            };

            // We create another window when launching the app because bad coding practices so we get this beauty :D
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(saveFileDialog, hwnd);

            // Show the save file dialog
            Windows.Storage.StorageFile file = await saveFileDialog.PickSaveFileAsync();

            if (file == null)
            {
                // User cancelled the dialog
                return;
            }

            // Export the transactions
            this.service.ExportTransactions([.. this.Transactions], file.Path, format);

            // Notify user of successful export
            this.ShowMessageBox("Export Successful", $"File saved: {file.Path}");
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
        private static bool ValidateNumericValue(string value)
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
        private static bool ValidateTotalValues(string minTotalValue, string maxTotalValue)
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
        private static bool ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return startDate.Value < endDate.Value;
            }

            return true;
        }

        /// <summary>
        /// Loads and filters transactions from the homepageService and applies sorting.
        /// </summary>
        public void LoadTransactions()
        {
            if (this.service == null)
            {
                throw new InvalidOperationException("Transaction homepageService is not initialized");
            }

            // Add null checks here for all ComboBoxItem properties to prevent null reference
            string transactionType = this.selectedTransactionType ?? "ALL";
            string sortBy = this.selectedSortBy ?? "Date";
            string sortOrder = this.selectedSortOrder ?? "ASC";

            // Validate MinTotalValue < MaxTotalValue
            if (!ValidateTotalValues(this.minTotalValue, this.maxTotalValue))
            {
                this.ShowMessageBox("Invalid Total Values", "Min Total Value must be less than Max Total Value.");
                return;
            }

            // Validate StartDate < EndDate
            if (!ValidateDateRange(this.startDate, this.endDate))
            {
                this.ShowMessageBox("Invalid Date Range", "Start Date must be earlier than End Date.");
                return;
            }

            DateTime startDate = this.startDate;
            DateTime endDate = this.endDate;

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
                ?? throw new InvalidOperationException("Transaction homepageService returned null");

            var transactionsSorted = transactions.OrderBy<TransactionLogTransaction, object>(t =>
            {
                return sortBy switch
                {
                    "Date" => t.Date,
                    "Stock Name" => t.StockName,
                    "Total Value" => t.TotalValue,
                    _ => throw new InvalidOperationException($"Invalid sort type: {sortBy}"),
                };
            });

            if (sortOrder == "DESC")
            {
                foreach (var transaction in transactionsSorted.Reverse())
                {
                    this.Transactions.Add(transaction);
                }
            }
            else
            {
                foreach (var transaction in transactionsSorted)
                {
                    this.Transactions.Add(transaction);
                }
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
