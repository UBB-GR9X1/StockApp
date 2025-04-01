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
        private ObservableCollection<Stock> _filteredAllStocks;
        private ObservableCollection<Stock> _filteredFavoriteStocks;
        private string _searchQuery;
        private string _selectedSortOption;

        public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public ICommand FavoriteCommand { get; }


        public ObservableCollection<Stock> FilteredAllStocks
        {
            get => _filteredAllStocks;
            private set
            {
                _filteredAllStocks = value;
                OnPropertyChanged("FilteredAllStocks");
            }
        }

        public ObservableCollection<Stock> FilteredFavoriteStocks
        {
            get => _filteredFavoriteStocks;
            private set
            {
                _filteredFavoriteStocks = value;
                OnPropertyChanged("FilteredFavoriteStocks");
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
            FilteredAllStocks = new ObservableCollection<Stock>(_service.GetAllStocks());
            FilteredFavoriteStocks = new ObservableCollection<Stock>(_service.GetFavoriteStocks());
            FavoriteCommand = new RelayCommand<Stock>(ToggleFavorite);
        }

        private void OnPropertyChanged(string propertyName)
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

        private void ToggleFavorite(Stock stock)
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
            FilteredAllStocks = new ObservableCollection<Stock>(_service.GetAllStocks());
            FilteredFavoriteStocks = new ObservableCollection<Stock>(_service.GetFavoriteStocks());
        }

        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            public event EventHandler CanExecuteChanged;

            public RelayCommand(Action<T> execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                if (parameter is T castParameter)
                {
                    _execute(castParameter);
                }
            }
        }



    }
}