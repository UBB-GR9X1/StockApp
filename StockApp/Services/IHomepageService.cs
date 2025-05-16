namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IHomepageService
    {
        //move to StocksService
        Task AddToFavoritesAsync(HomepageStock stock);

        Task RemoveFromFavoritesAsync(HomepageStock stock);

        Task<List<HomepageStock>> GetFilteredAndSortedStocksAsync(string query, string sortOption, bool favoritesOnly);

    }
}
