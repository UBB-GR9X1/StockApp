namespace StockApp.Services
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    class HomepageService : IHomepageService
    {
        private readonly IHomepageStocksRepository _repo;
        public ObservableCollection<IHomepageStock> FavoriteStocks { get; private set; }
        public ObservableCollection<IHomepageStock> AllStocks { get; private set; }
        public ObservableCollection<IHomepageStock> FilteredAllStocks { get; private set; }
        public ObservableCollection<IHomepageStock> FilteredFavoriteStocks { get; private set; }

        public HomepageService()
        {
            _repo = new HomepageStocksRepository();
            var stocks = _repo.LoadStocks();
            AllStocks = new ObservableCollection<IHomepageStock>(stocks);
            FavoriteStocks = new ObservableCollection<IHomepageStock>(stocks.Where(stock => stock.IsFavorite).ToList());
        }

        public ObservableCollection<IHomepageStock> GetFavoriteStocks()
        {
            return FavoriteStocks;
        }

        public ObservableCollection<IHomepageStock> GetAllStocks()
        {
            return AllStocks;
        }

        public void FilterStocks(string query)
        {
            FilteredAllStocks = new ObservableCollection<IHomepageStock>(AllStocks
                .Where(stock => stock.Name.ToLower().Contains(query.ToLower()) ||
                                stock.Symbol.ToLower().Contains(query.ToLower()))
                .ToList());

            FilteredFavoriteStocks = new ObservableCollection<IHomepageStock>(FavoriteStocks
                .Where(stock => stock.Name.ToLower().Contains(query.ToLower()) ||
                                stock.Symbol.ToLower().Contains(query.ToLower()))
                .ToList());
        }

        public void SortStocks(string sortOption)
        {
            if (FilteredAllStocks == null || FilteredFavoriteStocks == null)
            {
                FilteredAllStocks = new ObservableCollection<IHomepageStock>(AllStocks);
                FilteredFavoriteStocks = new ObservableCollection<IHomepageStock>(FavoriteStocks);
            }

            switch (sortOption)
            {
                case "Sort by Name":
                    FilteredAllStocks = new ObservableCollection<IHomepageStock>(FilteredAllStocks.OrderBy(stock => stock.Name).ToList());
                    FilteredFavoriteStocks = new ObservableCollection<IHomepageStock>(FilteredFavoriteStocks.OrderBy(stock => stock.Name).ToList());
                    break;
                case "Sort by Price":
                    FilteredAllStocks = new ObservableCollection<IHomepageStock>(
                        FilteredAllStocks.OrderBy(stock => stock.Price).ToList()
                    );
                    FilteredFavoriteStocks = new ObservableCollection<IHomepageStock>(
                        FilteredFavoriteStocks.OrderBy(stock => stock.Price).ToList()
                    );
                    break;
                case "Sort by Change":
                    FilteredAllStocks = new ObservableCollection<IHomepageStock>(
                        FilteredAllStocks.OrderBy(stock =>
                        {
                            // Remove '+' and '%' characters and parse to decimal
                            string cleanValue = stock.Change.Replace("+", "").Replace("%", "");
                            decimal.TryParse(cleanValue, out var change);
                            return change;
                        }).ToList()
                    );
                    FilteredFavoriteStocks = new ObservableCollection<IHomepageStock>(
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

        public void RemoveFromFavorites(IHomepageStock stock)
        {
            _repo.RemoveFromFavorites(stock);
            FavoriteStocks.Remove(stock);
            stock.IsFavorite = false;
        }

        public void AddToFavorites(IHomepageStock stock)
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
