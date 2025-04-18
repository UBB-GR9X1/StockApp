namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IHomepageStocksRepository
    {
        string GetUserCnp();

        bool IsGuestUser(string userCnp);

        public List<HomepageStock> LoadStocks();

        void AddToFavorites(HomepageStock stock);

        void RemoveFromFavorites(HomepageStock stock);

        public List<int> GetStockHistory(string stockName);

        void CreateUserProfile();
    }
}
