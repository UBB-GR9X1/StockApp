namespace StockApp.Services
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    class HomepageService : IHomepageService
    {
        private readonly IHomepageStocksRepository _repo;

        /// <summary>
        /// Represents a collection of stocks displayed on the homepage.
        /// </summary>
        public ObservableCollection<HomepageStock> FavoriteStocks { get; private set; }

        /// <summary>
        /// Represents a collection of all stocks available in the application.
        /// </summary>
        public ObservableCollection<HomepageStock> AllStocks { get; private set; }

        /// <summary>
        /// Represents a collection of stocks filtered based on user input.
        /// </summary>
        public ObservableCollection<HomepageStock> FilteredAllStocks { get; private set; }

        /// <summary>
        /// Represents a collection of favorite stocks filtered based on user input.
        /// </summary>
        public ObservableCollection<HomepageStock> FilteredFavoriteStocks { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomepageService"/> class.
        /// </summary>
        /// <param name="repo"></param>
        public HomepageService(IHomepageStocksRepository repo)
        {
            _repo = repo;
            var stocks = _repo.LoadStocks();
            AllStocks = new ObservableCollection<HomepageStock>(stocks);
            FavoriteStocks = new ObservableCollection<HomepageStock>(stocks.Where(s => s.IsFavorite).ToList());
        }

        /// <summary>
        /// Default constructor for the <see cref="HomepageService"/> class.
        /// </summary>
        public HomepageService() : this(new HomepageStocksRepository()) { }

        /// <summary>
        /// Returns a collection of stocks that are marked as favorites.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<HomepageStock> GetFavoriteStocks()
        {
            return FavoriteStocks;
        }

        /// <summary>
        /// Returns a collection of all stocks available in the application.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<HomepageStock> GetAllStocks()
        {
            return AllStocks;
        }

        /// <summary>
        /// Filters the stocks based on the provided query string.
        /// </summary>
        /// <param name="query"></param>
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

        /// <summary>
        /// Sorts the stocks based on the provided sort option.
        /// </summary>
        /// <param name="sortOption"></param>
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

        /// <summary>
        /// Removes a stock from the favorites list and updates its status.
        /// </summary>
        /// <param name="stock"></param>
        public void RemoveFromFavorites(HomepageStock stock)
        {
            _repo.RemoveFromFavorites(stock);
            FavoriteStocks.Remove(stock);
            stock.IsFavorite = false;
        }

        /// <summary>
        /// Adds a stock to the favorites list and updates its status.
        /// </summary>
        /// <param name="stock"></param>
        public void AddToFavorites(HomepageStock stock)
        {
            _repo.AddToFavorites(stock);
            FavoriteStocks.Add(stock);
            stock.IsFavorite = true;
        }

        /// <summary>
        /// Checks if the user is a guest user.
        /// </summary>
        /// <returns></returns>
        public bool IsGuestUser()
        {
            return _repo.IsGuestUser(_repo.GetUserCnp());
        }

        /// <summary>
        /// Returns the user's CNP (Personal Numeric Code).
        /// </summary>
        /// <returns></returns>
        public string GetUserCNP()
        {
            return _repo.GetUserCnp();
        }

        /// <summary>
        /// Creates a user profile if it doesn't exist.
        /// </summary>
        public void CreateUserProfile()
        {
            _repo.CreateUserProfile();
        }
    }
}
