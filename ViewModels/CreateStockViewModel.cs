using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StockApp.ViewModels.Tests")]

namespace StockApp.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Services;

    public class CreateStockViewModel : INotifyPropertyChanged
    {
        private readonly ICreateStockService stockService;
        private readonly IUserService userService;
        private string stockName;
        private string stockSymbol;
        private string authorCnp;
        private string message;
        private bool suppressValidation;
        private bool isAdmin;
        private bool isInputValid;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the command to create a new stock entry.
        /// </summary>
        public ICommand CreateStockCommand { get; }

        public CreateStockViewModel(ICreateStockService stockService, IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            this.CreateStockCommand = new RelayCommand(this.CreateStock, this.CanCreateStock);
            this.suppressValidation = true;
            this.StockName = string.Empty;
            this.StockSymbol = string.Empty;
            this.AuthorCnp = string.Empty;
            this.suppressValidation = false;
            this.Initialize();
        }

        private async void Initialize()
        {
            this.IsAdmin = await this.CheckIfUserIsAdmin();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current user is an administrator.
        /// </summary>
        public bool IsAdmin
        {
            get => this.isAdmin;
            set
            {
                if (this.isAdmin != value)
                {
                    this.isAdmin = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the stock to create.
        /// </summary>
        public string StockName
        {
            get => this.stockName;
            set
            {
                if (this.stockName != value)
                {
                    this.stockName = value;
                    this.ValidateInputs();
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the symbol of the stock to create.
        /// </summary>
        public string StockSymbol
        {
            get => this.stockSymbol;
            set
            {
                if (this.stockSymbol != value)
                {
                    this.stockSymbol = value;
                    this.ValidateInputs();
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the CNP identifier of the author.
        /// </summary>
        public string AuthorCnp
        {
            get => this.authorCnp;
            set
            {
                if (this.authorCnp != value)
                {
                    this.authorCnp = value;
                    this.ValidateInputs();
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the validation or homepageService message to display.
        /// </summary>
        public string Message
        {
            get => this.message;
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current inputs are valid for creating a stock.
        /// </summary>
        public bool IsInputValid
        {
            get => this.isInputValid;
            private set
            {
                if (this.isInputValid != value)
                {
                    this.isInputValid = value;
                    this.OnPropertyChanged();

                    // Notify the command that it can execute or not
                    ((RelayCommand)this.CreateStockCommand).RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Validates all input fields and sets <see cref="IsInputValid"/> and <see cref="Message"/> accordingly.
        /// </summary>
        private void ValidateInputs()
        {
            if (this.suppressValidation)
            {
                return;
            }

            // Validate stock name presence and format
            if (string.IsNullOrWhiteSpace(this.StockName))
            {
                this.Message = "Stock Name is required!";
                this.IsInputValid = false;
                return;
            }

            if (!Regex.IsMatch(this.StockName, @"^[A-Za-z ]{1,20}$"))
            {
                // Only letters and spaces, up to 20 characters
                this.Message = "Stock Name must be max 20 characters and contain only letters & spaces!";
                this.IsInputValid = false;
                return;
            }

            // Validate stock symbol presence and format
            if (string.IsNullOrWhiteSpace(this.StockSymbol))
            {
                this.Message = "Stock Symbol is required!";
                this.IsInputValid = false;
                return;
            }

            if (!Regex.IsMatch(this.StockSymbol, @"^[A-Za-z0-9]{1,5}$"))
            {
                // Alphanumeric only, up to 5 characters
                this.Message = "Stock Symbol must be alphanumeric and max 5 characters!";
                this.IsInputValid = false;
                return;
            }

            // Validate CNP presence and format
            if (string.IsNullOrWhiteSpace(this.AuthorCnp))
            {
                this.Message = "Author CNP is required!";
                this.IsInputValid = false;
                return;
            }

            if (!Regex.IsMatch(this.AuthorCnp, @"^\d{13}$"))
            {
                // Exactly 13 digits required
                this.Message = "Author CNP must be exactly 13 digits!";
                this.IsInputValid = false;
                return;
            }

            this.Message = string.Empty;
            this.IsInputValid = true;
        }

        private bool CanCreateStock(object? obj) => this.isAdmin && this.IsInputValid;

        private async void CreateStock(object obj)
        {
            if (!this.CanCreateStock(null))
            {
                return;
            }

            this.Message = await this.stockService.AddStockAsync(this.StockName, this.StockSymbol, this.AuthorCnp);

            if (this.Message == "Stock added successfully with initial value!")
            {
                this.suppressValidation = true;
                this.StockName = string.Empty;
                this.StockSymbol = string.Empty;
                this.AuthorCnp = string.Empty;
                this.suppressValidation = false;
            }
        }

        /// <summary>
        /// Checks whether the current user has administrative privileges.
        /// </summary>
        /// <returns>True if the user is an admin; otherwise false.</returns>
        protected async Task<bool> CheckIfUserIsAdmin()
        {
            User user = await this.userService.GetCurrentUserAsync();
            // This method should check if the user is an admin.
            // For now, let's assume the user is an admin.
            if (!user.IsModerator)
            {
                this.Message = "You are a guest user and cannot create stocks!";
            }

            return !user.IsModerator;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed. Auto-supplied if omitted.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
