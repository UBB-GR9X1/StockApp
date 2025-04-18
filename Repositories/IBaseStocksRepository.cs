namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IBaseStocksRepository
    {
        void AddStock(BaseStock stock, int initialPrice = 100);

        List<BaseStock> GetAllStocks();
    }
}
