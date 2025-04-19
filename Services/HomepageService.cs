namespace StockApp.Services
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    public class HomepageService : IHomepageService
    {
        private readonly IHomepageStocksRepository repo;
        public ObservableCollection<HomepageStock> FavoriteStocks { get; private set; }
        public ObservableCollection<HomepageStock> AllStocks { get; private set; }
        public ObservableCollection<HomepageStock> FilteredAllStocks { get; private set; }
        public ObservableCollection<HomepageStock> FilteredFavoriteStocks { get; private set; }

        public HomepageService(IHomepageStocksRepository repo)
        {
            this.repo = repo;
            var stocks = this.repo.LoadStocks();
            this.AllStocks = new ObservableCollection<HomepageStock>(stocks);
            this.FavoriteStocks = new ObservableCollection<HomepageStock>(stocks.Where(s => s.IsFavorite).ToList());
        }

        public HomepageService() : this(new HomepageStocksRepository()) { }

        public ObservableCollection<HomepageStock> GetFavoriteStocks()
        {
            return this.FavoriteStocks;
        }

        public ObservableCollection<HomepageStock> GetAllStocks()
        {
            return this.AllStocks;
        }

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

        public void RemoveFromFavorites(HomepageStock stock)
        {
            this.repo.RemoveFromFavorites(stock);
            this.FavoriteStocks.Remove(stock);
            stock.IsFavorite = false;
        }

        public void AddToFavorites(HomepageStock stock)
        {
            this.repo.AddToFavorites(stock);
            this.FavoriteStocks.Add(stock);
            stock.IsFavorite = true;
        }

        public bool IsGuestUser()
        {
            return this.repo.IsGuestUser(this.repo.GetUserCnp());
        }

        public string GetUserCNP()
        {
            return this.repo.GetUserCnp();
        }

        public void CreateUserProfile()
        {
            this.repo.CreateUserProfile();
        }
    }
}
