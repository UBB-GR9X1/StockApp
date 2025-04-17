namespace StockApp.Repository
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IHomepageStocksRepository
    {
        string GetUserCnp();

        bool IsGuestUser(string userCnp);

        public IReadOnlyList<IHomepageStock> LoadStocks();

        void AddToFavorites(IHomepageStock stock);

        void RemoveFromFavorites(IHomepageStock stock);

        public IReadOnlyList<int> GetStockHistory(string stockName);
    }
}
