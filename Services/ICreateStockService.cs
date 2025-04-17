namespace StockApp.Service
{
    public interface ICreateStockService
    {
        bool CheckIfUserIsGuest();

        string AddStock(string stockName, string stockSymbol, string authorCNP);
    }
}
