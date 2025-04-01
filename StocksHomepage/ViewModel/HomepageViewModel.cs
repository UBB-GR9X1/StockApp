using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Xml.Linq;
using StocksHomepage.Model;
using StocksHomepage.Service;

// return new global::StocksHomepage.Model.Stock { Change="", isFavorite=false, Name="", Price="", Symbol="" };

namespace StocksHomepage.ViewModel
{
    public class HomepageViewModel : INotifyPropertyChanged
    {
        private HomepageService _service;
        private ObservableCollection<HomepageStock> _filteredAllStocks;
        private ObservableCollection<HomepageStock> _filteredFavoriteStocks;
        private string _searchQuery;
        private string _selectedSortOption;
        private bool _isGuestUser;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand FavoriteCommand { get; }

        public ObservableCollection<HomepageStock> FilteredAllStocks
        {
            get => _filteredAllStocks;
            private set
            {
                _filteredAllStocks = value;
                OnPropertyChanged("FilteredAllStocks");
            }
        }

        public ObservableCollection<HomepageStock> FilteredFavoriteStocks
        {
            get => _filteredFavoriteStocks;
            private set
            {
                _filteredFavoriteStocks = value;
                OnPropertyChanged("FilteredFavoriteStocks");
            }
        }

        public bool IsGuestUser
        {
            get => _isGuestUser;
            set
            {
                if (_isGuestUser != value)
                {
                    //Console.WriteLine("IsGuestUser before: " + _isGuestUser);
                    _isGuestUser = value;
                    OnPropertyChanged();
                    //Console.WriteLine("IsGuestUser after: " + _isGuestUser);
                }
            }
        }

        public bool CheckUserStatus(string userCNP)
        {
            //return false;
            return _service.IsGuestUser(userCNP);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged("SearchQuery");
                ApplyFilter();
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged("SelectedSortOption");
                ApplySort();
            }
        }

        public HomepageViewModel()
        {
            _service = new HomepageService();
            //Console.WriteLine("IsGuestUser: " + IsGuestUser);
            //IsGuestUser = CheckUserStatus("12345678905");
            //Console.WriteLine("IsGuestUser: " + IsGuestUser);
            FilteredAllStocks = new ObservableCollection<HomepageStock>(_service.GetAllStocks());
            FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(_service.GetFavoriteStocks());
            FavoriteCommand = new RelayCommand(obj => ToggleFavorite(obj as HomepageStock), CanToggleFavorite);

            //FavoriteCommand = new RelayCommand(ToggleFavorite, CanToggleFavorite);
        }

        private bool CanToggleFavorite(object obj)
        {
            return IsGuestUser;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ApplyFilter()
        {
            _service.FilterStocks(SearchQuery);
            FilteredAllStocks = _service.FilteredAllStocks;
            FilteredFavoriteStocks = _service.FilteredFavoriteStocks;
        }

        public void ApplySort()
        {
            _service.SortStocks(SelectedSortOption);
            FilteredAllStocks = _service.FilteredAllStocks;
            FilteredFavoriteStocks = _service.FilteredFavoriteStocks;
        }

        private void ToggleFavorite(HomepageStock stock)
        {
            if (stock.isFavorite)
            {
                _service.RemoveFromFavorites(stock);
                RefreshStocks();
            }
            else
            {
                _service.AddToFavorites(stock);
                RefreshStocks();
            }
        }

        private void RefreshStocks()
        {
            FilteredAllStocks = new ObservableCollection<HomepageStock>(_service.GetAllStocks());
            FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(_service.GetFavoriteStocks());
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool> _canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute?.Invoke(parameter) ?? true;
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }



        }
    }
}