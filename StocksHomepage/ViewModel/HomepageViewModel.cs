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

// return new global::StocksHomepage.Model.Stock { Change="", isFavorite=false, Name="", Price="", Symbol="" };

namespace StocksHomepage.ViewModel
{
    public class HomepageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Stock> FavoriteStocks { get; private set; }
        public ObservableCollection<Stock> AllStocks { get; private set; }

        public ObservableCollection<Stock> FilteredAllStocks { get; private set; }
        public ObservableCollection<Stock> FilteredFavoriteStocks { get; private set; }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterStocks();
            }
        }

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
                FilterStocks();
            }
        }

        public ICommand FavoriteCommand { get; }

        public HomepageViewModel()
        {
            FavoriteStocks = new ObservableCollection<Stock>();
            AllStocks = new ObservableCollection<Stock>();

            FilteredAllStocks = new ObservableCollection<Stock>();
            FilteredFavoriteStocks = new ObservableCollection<Stock>();

            LoadStocks();
            FavoriteCommand = new RelayCommand<Stock>(ToggleFavorite);
        }

        private void LoadStocks()
        {
            // Populate stocks
            var initialFavorites = new List<Stock>
        {
            new Stock { Symbol = "AAPL", Name = "Apple Inc.", Price = "$175.00", Change = "+1.2%", isFavorite = true },
            new Stock { Symbol = "MSFT", Name = "Microsoft Corp.", Price = "$320.00", Change = "-0.8%", isFavorite = true },
            new Stock { Symbol = "NVDA", Name = "NVIDIA Corporation", Price = "$600.00", Change = "+3.1%", isFavorite = true },
            new Stock { Symbol = "TSM", Name = "Taiwan Semiconductor", Price = "$110.00", Change = "+1.5%" , isFavorite = true }
        };

            var initialStocks = new List<Stock>
        {
            new Stock { Symbol = "GOOGL", Name = "Alphabet Inc.", Price = "$2800.00", Change = "+0.5%", isFavorite = false },
                new Stock { Symbol = "AMZN", Name = "Amazon.com Inc.", Price = "$3500.00", Change = "-1.0%", isFavorite = false },
                new Stock { Symbol = "TSLA", Name = "Tesla Inc.", Price = "$700.00", Change = "+2.3%", isFavorite = false },
                new Stock { Symbol = "META", Name = "Meta Platforms, Inc.", Price = "$340.00", Change = "+0.8%", isFavorite = false },
                new Stock { Symbol = "DIS", Name = "The Walt Disney Company", Price = "$190.00", Change = "-2.0%", isFavorite = false },
                new Stock { Symbol = "NFLX", Name = "Netflix, Inc.", Price = "$500.00", Change = "+2.8%", isFavorite = false },
                new Stock { Symbol = "INTC", Name = "Intel Corporation", Price = "$50.00", Change = "-0.5%", isFavorite = false },
                new Stock { Symbol = "CSCO", Name = "Cisco Systems, Inc.", Price = "$55.00", Change = "+0.2%", isFavorite = false },
                new Stock { Symbol = "QCOM", Name = "QUALCOMM Incorporated", Price = "$150.00", Change = "-0.1%", isFavorite = false },
                new Stock { Symbol = "IBM", Name = "International Business Machines Corporation", Price = "$120.00", Change = "+0.3%", isFavorite = false },
                new Stock { Symbol = "ORCL", Name = "Oracle Corporation", Price = "$80.00", Change = "-0.4%", isFavorite = false },
                new Stock { Symbol = "ADBE", Name = "Adobe Inc.", Price = "$600.00", Change = "+1.0%", isFavorite = false },
                new Stock { Symbol = "CRM", Name = "Salesforce.com, inc.", Price = "$250.00", Change = "-0.3%", isFavorite = false },
                new Stock { Symbol = "NOW", Name = "ServiceNow, Inc.", Price = "$500.00", Change = "+0.7%", isFavorite = false },
                new Stock { Symbol = "SAP", Name = "SAP SE", Price = "$150.00", Change = "-0.2%", isFavorite = false },
                new Stock { Symbol = "UBER", Name = "Uber Technologies, Inc.", Price = "$40.00", Change = "+0.9%", isFavorite = false },
                new Stock { Symbol = "LYFT", Name = "Lyft, Inc.", Price = "$50.00", Change = "-0.6%", isFavorite = false },
                new Stock { Symbol = "ZM", Name = "Zoom Video Communications, Inc.", Price = "$200.00", Change = "+1.2%", isFavorite = false },
                new Stock { Symbol = "DOCU", Name = "DocuSign, Inc.", Price = "$150.00", Change = "-0.8%", isFavorite = false }
        };

            foreach (var stock in initialFavorites)
                FavoriteStocks.Add(stock);
            foreach (var stock in initialStocks)
                AllStocks.Add(stock);

            FilterStocks(); // Initialize filtered lists
        }

        private void ToggleFavorite(Stock stock)
        {
            if (stock.isFavorite)
            {
                FavoriteStocks.Remove(stock);
                stock.isFavorite = false;
                AllStocks.Add(stock);
            }
            else
            {
                AllStocks.Remove(stock);
                stock.isFavorite = true;
                FavoriteStocks.Add(stock);
            }
            FilterStocks();
        }

        private void FilterStocks()
        {
            var query = SearchQuery.ToLower();

            var filteredAll = AllStocks.Where(stock =>
                stock.Name.ToLower().Contains(query) || stock.Symbol.ToLower().Contains(query)).ToList();

            var filteredFavorites = FavoriteStocks.Where(stock =>
                stock.Name.ToLower().Contains(query) || stock.Symbol.ToLower().Contains(query)).ToList();

            switch (SelectedSortOption)
            {
                case "Sort by Name":
                    filteredAll = filteredAll.OrderBy(stock => stock.Name).ToList();
                    filteredFavorites = filteredFavorites.OrderBy(stock => stock.Name).ToList();
                    break;
                case "Sort by Price":
                    filteredAll = filteredAll.OrderBy(stock => decimal.Parse(stock.Price.Trim('$'))).ToList();
                    filteredFavorites = filteredFavorites.OrderBy(stock => decimal.Parse(stock.Price.Trim('$'))).ToList();
                    break;
                case "Sort by Change":
                    filteredAll = filteredAll.OrderBy(stock => decimal.Parse(stock.Change.Trim('%'))).ToList();
                    filteredFavorites = filteredFavorites.OrderBy(stock => decimal.Parse(stock.Change.Trim('%'))).ToList();
                    break;
            }

            // Clear and update filtered collections
            FilteredAllStocks.Clear();
            FilteredFavoriteStocks.Clear();

            foreach (var stock in filteredAll)
                FilteredAllStocks.Add(stock);
            foreach (var stock in filteredFavorites)
                FilteredFavoriteStocks.Add(stock);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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