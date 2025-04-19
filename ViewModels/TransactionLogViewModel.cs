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

    public class TransactionLogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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

        public event Action<string, string> ShowMessageBoxRequested;

        public ObservableCollection<TransactionLogTransaction> Transactions { get; set; } = new ObservableCollection<TransactionLogTransaction>();

        public string StockNameFilter
        {
            get => this.stockNameFilter;
            set { this.stockNameFilter = value; this.OnPropertyChanged(nameof(this.StockNameFilter)); }
        }

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

        public string SelectedExportFormat
        {
            get => this.selectedExportFormat;
            set
            {
                this.selectedExportFormat = value;
                this.OnPropertyChanged(nameof(this.SelectedExportFormat));
            }
        }

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
                    this.ShowMessageBox("Invalid Input", "Min Total Value must be a valid number.");
                }
            }
        }

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
                    this.ShowMessageBox("Invalid Input", "Max Total Value must be a valid number.");
                }
            }
        }

        public DateTime? StartDate
        {
            get => this.startDate;
            set { this.startDate = value; this.OnPropertyChanged(nameof(this.StartDate)); this.LoadTransactions(); }
        }

        public DateTime? EndDate
        {
            get => this.endDate;
            set { this.endDate = value; this.OnPropertyChanged(nameof(this.EndDate)); this.LoadTransactions(); }
        }

        public ICommand SearchCommand { get; }
        public ICommand ExportCommand { get; }

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

        public TransactionLogViewModel()
          : this(new TransactionLogService(new TransactionRepository()))
        { }

        private void Search()
        {
            this.LoadTransactions();
        }

        private async Task Export()
        {
            string format = this.selectedExportFormat.ToString();
            string fileName = "transactions";

            // Save the file to the user's Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = Path.Combine(documentsPath, $"{fileName}.{format.ToLower()}");

            // Export the transactions
            this.service.ExportTransactions(this.Transactions.ToList(), fullPath, format);

            // ShowMessageBox("Export Successful", $"File saved successfully to: {documentsPath}");
        }

        // Show the message box for feedback
        public void ShowMessageBox(string title, string content)
        {
            this.ShowMessageBoxRequested?.Invoke(title, content);
        }

        // Validation for numeric values (MinTotalValue & MaxTotalValue)
        private bool ValidateNumericValue(string value)
        {
            return int.TryParse(value, out _); // Check if the value is a valid integer
        }

        // Validate MinTotalValue < MaxTotalValue
        private bool ValidateTotalValues(string minTotalValue, string maxTotalValue)
        {
            if (int.TryParse(minTotalValue, out int min) && int.TryParse(maxTotalValue, out int max))
            {
                return min < max; // Check if min is less than max
            }
            return true; // Return true if validation is not applicable (e.g., empty fields)
        }

        // Validate StartDate < EndDate
        private bool ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return startDate.Value < endDate.Value; // Ensure start date is before end date
            }
            return true; // Return true if validation is not applicable (e.g., empty fields)
        }

        public void LoadTransactions()
        {
            if (this.service == null)
                throw new InvalidOperationException("Transaction service is not initialized");

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
                EndDate = endDate
            };

            filterCriteria.Validate();

            var transactions = this.service.GetFilteredTransactions(filterCriteria)
                ?? throw new InvalidOperationException("Transaction service returned null");

            foreach (var transaction in transactions)
            {
                this.Transactions.Add(transaction);
            }

            this.SortTransactions(sortBy, sortOrder == "ASC");
        }

        private void SortTransactions(string sortBy, bool isAscending)
        {
            var sortedTransactions = this.service.SortTransactions(this.Transactions.ToList(), sortBy, isAscending);

            this.Transactions.Clear();
            foreach (var transaction in sortedTransactions)
            {
                this.Transactions.Add(transaction);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
