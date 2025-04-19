namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IStockPageRepository
    {
        StockPageUser User { get; }

        bool IsGuest { get; }

        void UpdateUserGems(int gems);

        void AddOrUpdateUserStock(string stockName, int quantity);

        void AddStockValue(string stockName, int price);

        StockPageStock GetStock(string stockName);

        List<int> GetStockHistory(string stockName);

        int GetOwnedStocks(string stockName);

        bool GetFavorite(string stockName);

        void ToggleFavorite(string stockName, bool state);
    }
}
