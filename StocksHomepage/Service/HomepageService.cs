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
using StocksHomepage.Repositories;
using System.Data.Common;
using System.Data.SQLite;

namespace StocksHomepage.Service
{
    class HomepageService
    {
        private HomepageStocksRepository _repo;
        public ObservableCollection<HomepageStock> FavoriteStocks { get; private set; }
        public ObservableCollection<HomepageStock> AllStocks { get; private set; }
        public ObservableCollection<HomepageStock> FilteredAllStocks { get; private set; }
        public ObservableCollection<HomepageStock> FilteredFavoriteStocks { get; private set; }

        public HomepageService()
        {
            _repo = new HomepageStocksRepository();
            var stocks = _repo.LoadStocks();
            AllStocks = new ObservableCollection<HomepageStock>(stocks);
            FavoriteStocks = new ObservableCollection<HomepageStock>(stocks.Where(stock => stock.isFavorite).ToList());
        }

        public ObservableCollection<HomepageStock> GetFavoriteStocks()
        {
            return FavoriteStocks;
        }

        public ObservableCollection<HomepageStock> GetAllStocks()
        {
            return AllStocks;
        }

        public void FilterStocks(string query)
        {
            FilteredAllStocks = new ObservableCollection<HomepageStock>(AllStocks
                .Where(stock => stock.Name.ToLower().Contains(query.ToLower()) ||
                                stock.Symbol.ToLower().Contains(query.ToLower()))
                .ToList());

            FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(FavoriteStocks
                .Where(stock => stock.Name.ToLower().Contains(query.ToLower()) ||
                                stock.Symbol.ToLower().Contains(query.ToLower()))
                .ToList());
        }

        public void SortStocks(string sortOption)
        {
            if (FilteredAllStocks == null || FilteredFavoriteStocks == null)
            {
                FilteredAllStocks = new ObservableCollection<HomepageStock>(AllStocks);
                FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(FavoriteStocks);
            }

            switch (sortOption)
            {
                case "Sort by Name":
                    FilteredAllStocks = new ObservableCollection<HomepageStock>(FilteredAllStocks.OrderBy(stock => stock.Name).ToList());
                    FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(FilteredFavoriteStocks.OrderBy(stock => stock.Name).ToList());
                    break;
                case "Sort by Price":
                    FilteredAllStocks = new ObservableCollection<HomepageStock>(
                        FilteredAllStocks.OrderBy(stock => stock.Price).ToList()
                    );
                    FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(
                        FilteredFavoriteStocks.OrderBy(stock => stock.Price).ToList()
                    );
                    break;
                case "Sort by Change":
                    FilteredAllStocks = new ObservableCollection<HomepageStock>(
                        FilteredAllStocks.OrderBy(stock =>
                            decimal.TryParse(stock.Change.Replace("%", ""), out var change) ? change : 0
                        ).ToList()
                    );
                    FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(
                        FilteredFavoriteStocks.OrderBy(stock =>
                            decimal.TryParse(stock.Change.Replace("%", ""), out var change) ? change : 0
                        ).ToList()
                    );
                    break;
            }
        }

        public void RemoveFromFavorites(HomepageStock stock)
        {
            _repo.RemoveFromFavorites(stock);
            FavoriteStocks.Remove(stock);
            stock.isFavorite = false;
        }

        public void AddToFavorites(HomepageStock stock)
        {
            _repo.AddToFavorites(stock);
            FavoriteStocks.Add(stock);
            stock.isFavorite = true;
        }
        public bool IsGuestUser()
        {
            return _repo.IsGuestUser(_repo.getCNP());
        }

        public string GetUserCNP()
        {
            return _repo.getCNP();
        }

        public void CreateUserProfile()
        {
            _repo.CreateUserProfile();
        }
    }
}
