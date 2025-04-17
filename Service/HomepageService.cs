namespace StockApp.Service
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repository;

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
            FavoriteStocks = new ObservableCollection<HomepageStock>(stocks.Where(stock => stock.IsFavorite).ToList());
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
                        {
                            // Remove '+' and '%' characters and parse to decimal
                            string cleanValue = stock.Change.Replace("+", "").Replace("%", "");
                            decimal.TryParse(cleanValue, out var change);
                            return change;
                        }).ToList()
                    );
                    FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(
                        FilteredFavoriteStocks.OrderBy(stock =>
                        {
                            // Remove '+' and '%' characters and parse to decimal  
                            string cleanValue = stock.Change.Replace("+", "").Replace("%", "");
                            decimal.TryParse(cleanValue, out var change);
                            return change;
                        }).ToList()
                    );
                    break;
            }
        }

        public void RemoveFromFavorites(HomepageStock stock)
        {
            _repo.RemoveFromFavorites(stock);
            FavoriteStocks.Remove(stock);
            stock.IsFavorite = false;
        }

        public void AddToFavorites(HomepageStock stock)
        {
            _repo.AddToFavorites(stock);
            FavoriteStocks.Add(stock);
            stock.IsFavorite = true;
        }
        public bool IsGuestUser()
        {
            return _repo.IsGuestUser(_repo.GetUserCnp());
        }

        public string GetUserCNP()
        {
            return _repo.GetUserCnp();
        }

        public void CreateUserProfile()
        {
            _repo.CreateUserProfile();
        }
    }
}
