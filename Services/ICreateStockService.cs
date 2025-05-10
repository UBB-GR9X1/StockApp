using System.Threading.Tasks;

namespace StockApp.Services
{
    public interface ICreateStockService
    {
        Task<string> AddStockAsync(string stockName, string stockSymbol, string authorCNP);

        Task<(bool success, string message)> CreateStockAsync(string stockName, string stockSymbol, string authorCnp);
    }
}