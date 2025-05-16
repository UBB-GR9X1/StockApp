namespace Common.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;

    public interface IHomepageService
    {
        //move to StocksService
        Task AddToFavoritesAsync(HomepageStock stock);

        Task RemoveFromFavoritesAsync(HomepageStock stock);

        Task<List<HomepageStock>> GetFilteredAndSortedStocksAsync(string query, string sortOption, bool favoritesOnly);

    }
}
