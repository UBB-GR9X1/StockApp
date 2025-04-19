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
        private string StockNameValue;
        private string StockSymbolValue;
        private string AuthorCnpValue;
        private string MessageValue;
        private bool SuppressValidationValue = false;
        private readonly ICreateStockService StockService;
        private bool IsAdminValue;
        private bool IsInputValidValue;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand CreateStockCommand { get; }

        public CreateStockViewModel(ICreateStockService stockService)
        {
            this.StockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            this.CreateStockCommand = new RelayCommand(this.CreateStock, this.CanCreateStock);
            this.IsAdmin = this.CheckIfUserIsAdmin();
        }

        public CreateStockViewModel()
            : this(new CreateStockService())
        {
        }

        public bool IsAdmin
        {
            get => this.IsAdminValue;
            set
            {
                if (this.IsAdminValue != value)
                {
                    this.IsAdminValue = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string StockName
        {
            get => this.StockNameValue;
            set
            {
                if (this.StockNameValue != value)
                {
                    this.StockNameValue = value;
                    this.ValidateInputs();
                    this.OnPropertyChanged();
                }
            }
        }

        public string StockSymbol
        {
            get => this.StockSymbolValue;
            set
            {
                if (this.StockSymbolValue != value)
                {
                    this.StockSymbolValue = value;
                    this.ValidateInputs();
                    this.OnPropertyChanged();
                }
            }
        }

        public string AuthorCnp
        {
            get => this.AuthorCnpValue;
            set
            {
                if (this.AuthorCnpValue != value)
                {
                    this.AuthorCnpValue = value;
                    this.ValidateInputs();
                    this.OnPropertyChanged();
                }
            }
        }

        public string Message
        {
            get => this.MessageValue;
            set
            {
                if (this.MessageValue != value)
                {
                    this.MessageValue = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsInputValid
        {
            get => this.IsInputValidValue;
            private set
            {
                if (this.IsInputValidValue != value)
                {
                    this.IsInputValidValue = value;
                }
            }
        }

        private void ValidateInputs()
        {
            if (this.SuppressValidationValue) return;

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

            this.Message = this.StockService.AddStock(this.StockName, this.StockSymbol, this.AuthorCnp);

            if (this.Message == "Stock added successfully with initial value!")
            {
                this.SuppressValidationValue = true;
                this.StockName = "";
                this.StockSymbol = "";
                this.AuthorCnp = "";
                this.SuppressValidationValue = false;
            }
        }

        protected virtual bool CheckIfUserIsAdmin()
        {
            // This method should check if the user is an admin.
            // For now, let's assume the user is an admin.
            if (this.StockService.CheckIfUserIsGuest())
                this.Message = "You are a guest user and cannot create stocks!";
            return !this.StockService.CheckIfUserIsGuest();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
