namespace StockApp.Services
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IHomepageService
    {
        Task AddToFavoritesAsync(HomepageStock stock);

        Task RemoveFromFavoritesAsync(HomepageStock stock);

        Task<ObservableCollection<HomepageStock>> GetFilteredAndSortedStocksAsync(string query, string sortOption, bool favoritesOnly);

    }
}
