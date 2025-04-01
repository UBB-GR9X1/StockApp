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
                _isAdmin = value;
                OnPropertyChanged();
                (CreateStockCommand as RelayCommand)?.OnCanExecuteChanged();
            }
        }

        public string StockName
        {
            get => _stockName;
            set
            {
                _stockName = value;
                ValidateInputs();
                OnPropertyChanged();
            }
        }

        public string StockSymbol
        {
            get => _stockSymbol;
            set
            {
                _stockSymbol = value;
                ValidateInputs();
                OnPropertyChanged();
            }
        }

        public string AuthorCNP
        {
            get => _authorCNP;
            set
            {
                _authorCNP = value;
                ValidateInputs();
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        private void ValidateInputs()
        {
            if (_suppressValidation) return;

            Message = string.Empty;

            if (string.IsNullOrWhiteSpace(StockName))
                Message = "Stock Name is required!";
            else if (!Regex.IsMatch(StockName, @"^[A-Za-z ]{1,20}$"))
                Message = "Stock Name must be max 20 characters and contain only letters & spaces!";
            else if (string.IsNullOrWhiteSpace(StockSymbol))
                Message = "Stock Symbol is required!";
            else if (!Regex.IsMatch(StockSymbol, @"^[A-Za-z0-9]{1,5}$"))
                Message = "Stock Symbol must be alphanumeric and max 5 characters!";
            else if (string.IsNullOrWhiteSpace(AuthorCNP))
                Message = "Author CNP is required!";
            else if (!Regex.IsMatch(AuthorCNP, @"^\d{13}$"))
                Message = "Author CNP must be exactly 13 digits!";

            (CreateStockCommand as RelayCommand)?.OnCanExecuteChanged();
        }

        private bool CanCreateStock(object obj)
        {
            return IsAdmin && string.IsNullOrEmpty(Message);
        }

        private void CreateStock(object obj)
        {
            if (!CanCreateStock(null)) return;

            Message = _stockService.AddStock(StockName, StockSymbol, AuthorCNP);

            if (Message == "Stock added successfully!")
            {
                _suppressValidation = true;
                StockName = "";
                StockSymbol = "";
                AuthorCNP = "";
                _suppressValidation = false;
            }
        }

        private bool CheckIfUserIsAdmin()
        {
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
