using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using System.Xml.Linq;
using StockApp;
using StocksHomepage.Model;

namespace StocksHomepage.Service
{
    class HomepageService
    {
        // This repo will be used after I connect to the database
        Repository _repo = Repository.Instance;
        // until then, I will use some temporary data
        public ObservableCollection<Stock> FavoriteStocks { get; private set; }
        public ObservableCollection<Stock> AllStocks { get; private set; }

        public ObservableCollection<Stock> FilteredAllStocks { get; private set; }
        public ObservableCollection<Stock> FilteredFavoriteStocks { get; private set; }

        public HomepageService()
        {
            FavoriteStocks = new ObservableCollection<Stock>()
                    {
                        new Stock { Symbol = "AAPL", Name = "Apple Inc.", Price = "$175.00", Change = "+1.2%", isFavorite = true },
                        new Stock { Symbol = "MSFT", Name = "Microsoft Corp.", Price = "$320.00", Change = "-0.8%", isFavorite = true },
                        new Stock { Symbol = "NVDA", Name = "NVIDIA Corporation", Price = "$600.00", Change = "+3.1%", isFavorite = true },
                        new Stock { Symbol = "TSM", Name = "Taiwan Semiconductor", Price = "$110.00", Change = "+1.5%" , isFavorite = true }
                    };
            AllStocks = new ObservableCollection<Stock>()
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
            //FilteredAllStocks = new ObservableCollection<Stock>(AllStocks);
            //FilteredFavoriteStocks = new ObservableCollection<Stock>(FavoriteStocks);

        }

        // Get all stocks (when repo will be done)
        //public List<StockApp.Model.Stock> GetAllStocks()
        //{
        //    return _repo.getStockList();
        //}

        // Get favorite stocks from service attribute
        public ObservableCollection<Stock> GetFavoriteStocks()
        {
            return FavoriteStocks;
        }

        // Get all stocks from service attribute
        public ObservableCollection<Stock> GetAllStocks()
        {
            return AllStocks;
        }

        // Filter stocks based on search query
        public void FilterStocks(string query)
        {
            FilteredAllStocks = new ObservableCollection<Stock>(AllStocks
                .Where(stock => stock.Name.ToLower().Contains(query.ToLower()) ||
                                stock.Symbol.ToLower().Contains(query.ToLower()))
                .ToList());

            FilteredFavoriteStocks = new ObservableCollection<Stock>(FavoriteStocks
                .Where(stock => stock.Name.ToLower().Contains(query.ToLower()) ||
                                stock.Symbol.ToLower().Contains(query.ToLower()))
                .ToList());
        }

        // Sort stocks based on the selected option
        public void SortStocks(string sortOption)
        {
            if (FilteredAllStocks == null || FilteredFavoriteStocks == null)
            {
                FilteredAllStocks = new ObservableCollection<Stock>(AllStocks);
                FilteredFavoriteStocks = new ObservableCollection<Stock>(FavoriteStocks);
            }

            switch (sortOption)
            {
                case "Sort by Name":
                    FilteredAllStocks = new ObservableCollection<Stock>(FilteredAllStocks.OrderBy(stock => stock.Name).ToList());
                    FilteredFavoriteStocks = new ObservableCollection<Stock>(FilteredFavoriteStocks.OrderBy(stock => stock.Name).ToList());
                    break;
                case "Sort by Price":
                    FilteredAllStocks = new ObservableCollection<Stock>(
                        FilteredAllStocks.OrderBy(stock =>
                            decimal.TryParse(stock.Price.Replace("$", ""), out var price) ? price : 0
                        ).ToList()
                    );
                    FilteredFavoriteStocks = new ObservableCollection<Stock>(
                        FilteredFavoriteStocks.OrderBy(stock =>
                            decimal.TryParse(stock.Price.Replace("$", ""), out var price) ? price : 0
                        ).ToList()
                    );
                    break;
                case "Sort by Change":
                    // sort by change percentage like we did for price, first the negative changes then the positive ones
                    FilteredAllStocks = new ObservableCollection<Stock>(
                        FilteredAllStocks.OrderBy(stock =>
                            decimal.TryParse(stock.Change.Replace("%", ""), out var change) ? change : 0
                        ).ToList()
                    );
                    FilteredFavoriteStocks = new ObservableCollection<Stock>(
                        FilteredFavoriteStocks.OrderBy(stock =>
                            decimal.TryParse(stock.Change.Replace("%", ""), out var change) ? change : 0
                        ).ToList()
                    );
                    break;
            }
        }


        // Add a stock to the favorites list
        public void AddToFavorites(Stock stock)
        {
            AllStocks.Remove(stock);
            FavoriteStocks.Add(stock);
            stock.isFavorite = true;
        }

        // Remove a stock from the favorites list
        public void RemoveFromFavorites(Stock stock)
        {
            FavoriteStocks.Remove(stock);
            AllStocks.Add(stock);
            stock.isFavorite = false;
        }

    }
} 
