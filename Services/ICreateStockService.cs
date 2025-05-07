using System.Threading.Tasks;

namespace StockApp.Services
{
    public interface ICreateStockService
    {
        bool CheckIfUserIsGuest();

        Task<string> AddStock(string stockName, string stockSymbol, string authorCNP);
    }
}
