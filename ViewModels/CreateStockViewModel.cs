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
        private string stockName;
        private string stockSymbol;
        private string authorCnp;
        private string message;
        private bool suppressValidation;
        private readonly ICreateStockService stockService;
        private bool isAdmin;
        private bool isInputValid;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand CreateStockCommand { get; }

        public CreateStockViewModel(ICreateStockService stockService)
        {
            this.stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            this.CreateStockCommand = new RelayCommand(this.CreateStock, this.CanCreateStock);
            this.IsAdmin = this.CheckIfUserIsAdmin();
        }

        public CreateStockViewModel()
            : this(new CreateStockService())
        {
        }

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

        public bool IsInputValid
        {
            get => this.isInputValid;
            private set
            {
                if (this.isInputValid != value)
                {
                    this.isInputValid = value;
                }
            }
        }

        private void ValidateInputs()
        {
            if (this.suppressValidation) return;

            this.Message = string.Empty;
            this.IsInputValid = true;

            if (string.IsNullOrWhiteSpace(this.StockName))
            {
                this.Message = "Stock Name is required!";
                this.IsInputValid = false;
            }
            else if (!Regex.IsMatch(this.StockName, @"^[A-Za-z ]{1,20}$"))
            {
                this.Message = "Stock Name must be max 20 characters and contain only letters & spaces!";
                this.IsInputValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.StockSymbol))
            {
                this.Message = "Stock Symbol is required!";
                this.IsInputValid = false;
            }
            else if (!Regex.IsMatch(this.StockSymbol, @"^[A-Za-z0-9]{1,5}$"))
            {
                this.Message = "Stock Symbol must be alphanumeric and max 5 characters!";
                this.IsInputValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.AuthorCnp))
            {
                this.Message = "Author CNP is required!";
                this.IsInputValid = false;
            }
            else if (!Regex.IsMatch(this.AuthorCnp, @"^\d{13}$"))
            {
                this.Message = "Author CNP must be exactly 13 digits!";
                this.IsInputValid = false;
            }
        }

        private bool CanCreateStock(object obj) => this.IsAdmin && this.IsInputValid;

        private void CreateStock(object obj)
        {
            if (!this.CanCreateStock(null)) return;

            this.Message = this.stockService.AddStock(this.StockName, this.StockSymbol, this.AuthorCnp);

            if (this.Message == "Stock added successfully with initial value!")
            {
                this.suppressValidation = true;
                this.StockName = "";
                this.StockSymbol = "";
                this.AuthorCnp = "";
                this.suppressValidation = false;
            }
        }

        protected virtual bool CheckIfUserIsAdmin()
        {
            // This method should check if the user is an admin.
            // For now, let's assume the user is an admin.
            if (this.stockService.CheckIfUserIsGuest())
                this.Message = "You are a guest user and cannot create stocks!";
            return !this.stockService.CheckIfUserIsGuest();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
