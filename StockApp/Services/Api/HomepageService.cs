namespace StockApp.Services.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Services;
    using StockApp.Repositories;

    public class HomepageService : IHomepageService
    {
        private readonly IHomepageStocksRepository homepageStocksRepo;

        public HomepageService(IHomepageStocksRepository homepageStocksRepository)
        {
            homepageStocksRepo = homepageStocksRepository;
        }

        public async Task<List<HomepageStock>> GetFilteredAndSortedStocksAsync(string query, string sortOption, bool favoritesOnly)
        {
            var allStocks = await homepageStocksRepo.GetAllStocksAsync();
            var filteredStocks = allStocks.Where(stock =>
                stock.StockDetails.Name.Contains(query, System.StringComparison.CurrentCultureIgnoreCase) ||
                stock.StockDetails.Symbol.Contains(query, System.StringComparison.CurrentCultureIgnoreCase));

            if (favoritesOnly)
            {
                filteredStocks = filteredStocks.Where(stock => stock.IsFavorite);
            }

            return sortOption switch
            {
                "Sort by Name" => filteredStocks.OrderBy(stock => stock.StockDetails.Name).ToList(),
                "Sort by Price" => filteredStocks.OrderBy(stock => stock.StockDetails.Price).ToList(),
                "Sort by Change" => filteredStocks.OrderBy(stock => stock.Change).ToList(),
                _ => filteredStocks.ToList()
            };
        }

        public async Task AddToFavoritesAsync(HomepageStock stock)
        {
            stock.IsFavorite = true;
            await homepageStocksRepo.UpdateStockAsync(stock.Id, stock);
        }

        public async Task RemoveFromFavoritesAsync(HomepageStock stock)
        {
            stock.IsFavorite = false;
            await homepageStocksRepo.UpdateStockAsync(stock.Id, stock);
        }
    }
}
