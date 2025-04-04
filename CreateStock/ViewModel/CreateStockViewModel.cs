using GemStore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StockApp.CreateStock.Command;
using StockApp.CreateStock.Service;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace StockApp.CreateStock.ViewModel
{
    internal class CreateStockViewModel : INotifyPropertyChanged
    {
        private string _stockName;
        private string _stockSymbol;
        private string _authorCNP;
        private string _message;
        private bool _suppressValidation = false;
        private readonly CreateStockService _stockService;
        private bool _isAdmin;
        private bool _isInputValid;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand CreateStockCommand { get; }

        public CreateStockViewModel()
        {
            _stockService = new CreateStockService();
            CreateStockCommand = new RelayCommand(CreateStock, CanCreateStock);
            IsAdmin = CheckIfUserIsAdmin();
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    OnPropertyChanged();
                    (CreateStockCommand as RelayCommand)?.OnCanExecuteChanged();
                }
            }
        }

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

        public string AuthorCNP
        {
            get => _authorCNP;
            set
            {
                if (_authorCNP != value)
                {
                    _authorCNP = value;
                    ValidateInputs();
                    OnPropertyChanged();
                }
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                    (CreateStockCommand as RelayCommand)?.OnCanExecuteChanged();
                }
            }
        }

        public bool IsInputValid
        {
            get => _isInputValid;
            private set
            {
                if (_isInputValid != value)
                {
                    _isInputValid = value;
                    (CreateStockCommand as RelayCommand)?.OnCanExecuteChanged();
                }
            }
        }

        private void ValidateInputs()
        {
            if (_suppressValidation) return;

            Message = string.Empty;
            IsInputValid = true;

            if (string.IsNullOrWhiteSpace(StockName))
            {
                Message = "Stock Name is required!";
                IsInputValid = false;
            }
            else if (!Regex.IsMatch(StockName, @"^[A-Za-z ]{1,20}$"))
            {
                Message = "Stock Name must be max 20 characters and contain only letters & spaces!";
                IsInputValid = false;
            }

            if (string.IsNullOrWhiteSpace(StockSymbol))
            {
                Message = "Stock Symbol is required!";
                IsInputValid = false;
            }
            else if (!Regex.IsMatch(StockSymbol, @"^[A-Za-z0-9]{1,5}$"))
            {
                Message = "Stock Symbol must be alphanumeric and max 5 characters!";
                IsInputValid = false;
            }

            if (string.IsNullOrWhiteSpace(AuthorCNP))
            {
                Message = "Author CNP is required!";
                IsInputValid = false;
            }
            else if (!Regex.IsMatch(AuthorCNP, @"^\d{13}$"))
            {
                Message = "Author CNP must be exactly 13 digits!";
                IsInputValid = false;
            }
        }

        private bool CanCreateStock(object obj) => IsAdmin && IsInputValid;

        private void CreateStock(object obj)
        {
            if (!CanCreateStock(null)) return;

            Message = _stockService.AddStock(StockName, StockSymbol, AuthorCNP);

            if (Message == "Stock added successfully with initial value!")
            {
                _suppressValidation = true;
                StockName = "";
                StockSymbol = "";
                AuthorCNP = "";
                _suppressValidation = false;
            }
        }

        protected virtual bool CheckIfUserIsAdmin()
        {
            // This method should check if the user is an admin.
            // For now, let's assume the user is an admin.
            bool isAdmin = true;

            return isAdmin;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
