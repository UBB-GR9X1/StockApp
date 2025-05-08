namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IHomepageStocksRepository
    {
        string GetUserCnp();

        bool IsGuestUser(string userCnp);

        List<HomepageStock> LoadStocks();

        void AddToFavorites(HomepageStock stock);

        void RemoveFromFavorites(HomepageStock stock);

        List<int> GetStockHistory(string stockName);

        void CreateUserProfile();
    }
}
