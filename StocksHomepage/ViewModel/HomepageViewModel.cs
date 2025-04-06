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
        private bool _isGuestUser = true;
        private string _guestButtonVisibility = "Visible";

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

        public string getUserCNP()
        {
            return _service.GetUserCNP();
        }

        public bool IsGuestUser
        {
            get => _isGuestUser;
            set
            {
                _isGuestUser = value;
                GuestButtonVisibility = _isGuestUser ? "Visible" : "Collapsed";
                OnPropertyChanged(nameof(IsGuestUser));
                OnPropertyChanged(nameof(CanModifyFavorites)); // Add this line
            }
        }

        public string GuestButtonVisibility
        {
            get { return _guestButtonVisibility; }
            set
            {
                _guestButtonVisibility = value;
                OnPropertyChanged(nameof(GuestButtonVisibility));
            }
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
            IsGuestUser = _service.IsGuestUser();
            FilteredAllStocks = new ObservableCollection<HomepageStock>(_service.GetAllStocks());
            FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(_service.GetFavoriteStocks());
            FavoriteCommand = new RelayCommand(obj => ToggleFavorite(obj as HomepageStock), CanToggleFavorite);

            //FavoriteCommand = new RelayCommand(ToggleFavorite, CanToggleFavorite);
        }

        public bool CanModifyFavorites
        {
            get => !_isGuestUser;
        }

        public bool CanToggleFavorite(object obj)
        {
            return !IsGuestUser;
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

        public void CreateUserProfile()
        {
            // Call the service to create a user profile
            _service.CreateUserProfile();

            // Update the guest status
            IsGuestUser = false;

            // Refresh the stocks to reflect new permissions
            RefreshStocks();
        }

        public void ApplySort()
        {
            _service.SortStocks(SelectedSortOption);
            FilteredAllStocks = _service.FilteredAllStocks;
            FilteredFavoriteStocks = _service.FilteredFavoriteStocks;
        }

        public void ToggleFavorite(HomepageStock stock)
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

        public void RefreshStocks()
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