namespace StockApp.Repository
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IBaseStocksRepository
    {
        void AddStock(IBaseStock stock, int initialPrice = 100);

        IReadOnlyList<IBaseStock> GetAllStocks();
    }
}
