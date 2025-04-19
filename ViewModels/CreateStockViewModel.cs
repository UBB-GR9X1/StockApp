using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StockApp.ViewModels.Tests")]
namespace StockApp.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using StockApp.Commands;
    using StockApp.Services;

    internal class CreateStockViewModel : INotifyPropertyChanged
    {
        private string _stockName;
        private string _stockSymbol;
        private string _authorCnp;
        private string _message;
        private bool _suppressValidation = false;
        private readonly ICreateStockService _stockService;
        private bool _isAdmin;
        private bool _isInputValid;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the command to create a new stock entry.
        /// </summary>
        public ICommand CreateStockCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user is an administrator.
        /// </summary>
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the stock to create.
        /// </summary>
        public string StockName
        {
            get => _stockName;
            set
            {
                if (_stockName != value)
                {
                    _stockName = value;
                    ValidateInputs();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the symbol of the stock to create.
        /// </summary>
        public string StockSymbol
        {
            get => _stockSymbol;
            set
            {
                if (_stockSymbol != value)
                {
                    _stockSymbol = value;
                    ValidateInputs();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the CNP identifier of the author.
        /// </summary>
        public string AuthorCnp
        {
            get => _authorCnp;
            set
            {
                if (_authorCnp != value)
                {
                    _authorCnp = value;
                    ValidateInputs();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the validation or service message to display.
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current inputs are valid for creating a stock.
        /// </summary>
        public bool IsInputValid
        {
            get => _isInputValid;
            private set
            {
                if (_isInputValid != value)
                {
                    _isInputValid = value;
                }
            }
        }

        public CreateStockViewModel(ICreateStockService stockService)
        {
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            CreateStockCommand = new RelayCommand(CreateStock, CanCreateStock);
            IsAdmin = CheckIfUserIsAdmin();
        }

        public CreateStockViewModel()
            : this(new CreateStockService())
        {
        }

        /// <summary>
        /// Validates all input fields and sets <see cref="IsInputValid"/> and <see cref="Message"/> accordingly.
        /// </summary>
        private void ValidateInputs()
        {
            if (_suppressValidation) return;

            Message = string.Empty;
            IsInputValid = true;

            // Validate stock name presence and format
            if (string.IsNullOrWhiteSpace(StockName))
            {
                Message = "Stock Name is required!";
                IsInputValid = false;
            }
            else if (!Regex.IsMatch(StockName, @"^[A-Za-z ]{1,20}$"))
            {
                // Only letters and spaces, up to 20 characters
                Message = "Stock Name must be max 20 characters and contain only letters & spaces!";
                IsInputValid = false;
            }

            // Validate stock symbol presence and format
            if (string.IsNullOrWhiteSpace(StockSymbol))
            {
                Message = "Stock Symbol is required!";
                IsInputValid = false;
            }
            else if (!Regex.IsMatch(StockSymbol, @"^[A-Za-z0-9]{1,5}$"))
            {
                // Alphanumeric only, up to 5 characters
                Message = "Stock Symbol must be alphanumeric and max 5 characters!";
                IsInputValid = false;
            }

            // Validate CNP presence and format
            if (string.IsNullOrWhiteSpace(AuthorCnp))
            {
                Message = "Author CNP is required!";
                IsInputValid = false;
            }
            else if (!Regex.IsMatch(AuthorCnp, @"^\d{13}$"))
            {
                // Exactly 13 digits required
                Message = "Author CNP must be exactly 13 digits!";
                IsInputValid = false;
            }
        }

        private bool CanCreateStock(object obj) => IsAdmin && IsInputValid;

        private void CreateStock(object obj)
        {
            if (!CanCreateStock(null)) return;

            Message = _stockService.AddStock(StockName, StockSymbol, AuthorCnp);

            if (Message == "Stock added successfully with initial value!")
            {
                // TODO: Clear inputs after successful creation without re-validating interim empty values
                _suppressValidation = true;
                StockName = string.Empty;
                StockSymbol = string.Empty;
                AuthorCnp = string.Empty;
                _suppressValidation = false;
            }
        }

        /// <summary>
        /// Checks whether the current user has administrative privileges.
        /// </summary>
        /// <returns>True if the user is an admin; otherwise false.</returns>
        protected virtual bool CheckIfUserIsAdmin()
        {
            // TODO: Implement real admin check logic instead of assuming guest status
            if (_stockService.CheckIfUserIsGuest())
                Message = "You are a guest user and cannot create stocks!";
            return !_stockService.CheckIfUserIsGuest();
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed. Auto-supplied if omitted.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
