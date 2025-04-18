using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Services
{
    public interface IStoreService
    {
        string GetCnp();

        bool IsGuest(string cnp);

        int GetUserGemBalance(string cnp);

        void UpdateUserGemBalance(string cnp, int newBalance);

        Task<string> BuyGems(string cnp, GemDeal deal, string selectedAccountId);

        Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId);
    }
}
