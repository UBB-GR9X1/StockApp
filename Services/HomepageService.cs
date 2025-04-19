namespace StockApp.Services
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    public class HomepageService : IHomepageService
    {
        /// <summary>
        /// Represents a collection of stocks displayed on the homepage.
        /// </summary>
        private readonly IHomepageStocksRepository repo;

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
            this.repo = repo;
            var stocks = this.repo.LoadStocks();
            this.AllStocks = new ObservableCollection<HomepageStock>(stocks);
            this.FavoriteStocks = new ObservableCollection<HomepageStock>(stocks.Where(s => s.IsFavorite).ToList());
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
            return this.FavoriteStocks;
        }

        /// <summary>
        /// Returns a collection of all stocks available in the application.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<HomepageStock> GetAllStocks()
        {
            return this.AllStocks;
        }

        /// <summary>
        /// Filters the stocks based on the provided query string.
        /// </summary>
        /// <param name="query"></param>
        public void FilterStocks(string query)
        {
            this.FilteredAllStocks = new ObservableCollection<HomepageStock>(this.AllStocks
                .Where(stock => stock.Name.ToLower().Contains(query.ToLower()) ||
                                stock.Symbol.ToLower().Contains(query.ToLower()))
                .ToList());

            this.FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(this.FavoriteStocks
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
            if (this.FilteredAllStocks == null || this.FilteredFavoriteStocks == null)
            {
                this.FilteredAllStocks = new ObservableCollection<HomepageStock>(this.AllStocks);
                this.FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(this.FavoriteStocks);
            }

            switch (sortOption)
            {
                case "Sort by Name":
                    this.FilteredAllStocks = new ObservableCollection<HomepageStock>(this.FilteredAllStocks.OrderBy(stock => stock.Name).ToList());
                    this.FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(this.FilteredFavoriteStocks.OrderBy(stock => stock.Name).ToList());
                    break;
                case "Sort by Price":
                    this.FilteredAllStocks = new ObservableCollection<HomepageStock>(
                        this.FilteredAllStocks.OrderBy(stock => stock.Price).ToList()
                    );
                    this.FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(
                        this.FilteredFavoriteStocks.OrderBy(stock => stock.Price).ToList()
                    );
                    break;
                case "Sort by Change":
                    this.FilteredAllStocks = new ObservableCollection<HomepageStock>(
                        this.FilteredAllStocks.OrderBy(stock =>
                        {
                            // Remove '+' and '%' characters and parse to decimal
                            string cleanValue = stock.Change.Replace("+", "").Replace("%", "");
                            decimal.TryParse(cleanValue, out var change);
                            return change;
                        }).ToList()
                    );
                    this.FilteredFavoriteStocks = new ObservableCollection<HomepageStock>(
                        this.FilteredFavoriteStocks.OrderBy(stock =>
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
            this.repo.RemoveFromFavorites(stock);
            this.FavoriteStocks.Remove(stock);
            stock.IsFavorite = false;
        }

        /// <summary>
        /// Adds a stock to the favorites list and updates its status.
        /// </summary>
        /// <param name="stock"></param>
        public void AddToFavorites(HomepageStock stock)
        {
            this.repo.AddToFavorites(stock);
            this.FavoriteStocks.Add(stock);
            stock.IsFavorite = true;
        }

        /// <summary>
        /// Checks if the user is a guest user.
        /// </summary>
        /// <returns></returns>
        public bool IsGuestUser()
        {
            return this.repo.IsGuestUser(this.repo.GetUserCnp());
        }

        /// <summary>
        /// Returns the user's CNP (Personal Numeric Code).
        /// </summary>
        /// <returns></returns>
        public string GetUserCNP()
        {
            return this.repo.GetUserCnp();
        }

        /// <summary>
        /// Creates a user profile if it doesn't exist.
        /// </summary>
        public void CreateUserProfile()
        {
            this.repo.CreateUserProfile();
        }
    }
}
