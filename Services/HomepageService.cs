namespace StockApp.Services
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class HomepageService : IHomepageService
    {
        private readonly IHomepageStocksRepository homepageStocksRepo;

        public HomepageService(IHomepageStocksRepository homepageStocksRepository)
        {
            this.homepageStocksRepo = homepageStocksRepository;
        }

        public async Task<ObservableCollection<HomepageStock>> GetFilteredAndSortedStocksAsync(string query, string sortOption, bool favoritesOnly)
        {
            var allStocks = await this.homepageStocksRepo.GetAllStocksAsync();
            var filteredStocks = allStocks.Where(stock =>
                stock.StockDetails.Name.Contains(query, System.StringComparison.CurrentCultureIgnoreCase) ||
                stock.StockDetails.Symbol.Contains(query, System.StringComparison.CurrentCultureIgnoreCase));

            if (favoritesOnly)
            {
                filteredStocks = filteredStocks.Where(stock => stock.IsFavorite);
            }

            return sortOption switch
            {
                "Sort by Name" => new ObservableCollection<HomepageStock>(filteredStocks.OrderBy(stock => stock.StockDetails.Name)),
                "Sort by Price" => new ObservableCollection<HomepageStock>(filteredStocks.OrderBy(stock => stock.StockDetails.Price)),
                "Sort by Change" => new ObservableCollection<HomepageStock>(filteredStocks.OrderBy(stock =>
                {
                    return stock.Change;
                })),
                _ => new ObservableCollection<HomepageStock>(filteredStocks)
            };
        }

        public async Task AddToFavoritesAsync(HomepageStock stock)
        {
            stock.IsFavorite = true;
            await this.homepageStocksRepo.UpdateStockAsync(stock.Id, stock);
        }

        public async Task RemoveFromFavoritesAsync(HomepageStock stock)
        {
            stock.IsFavorite = false;
            await this.homepageStocksRepo.UpdateStockAsync(stock.Id, stock);
        }
    }
}
