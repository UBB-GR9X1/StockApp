namespace StockApp.Services
{
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IStoreService
    {
        Task<string> GetCnpAsync();

        Task<bool> IsGuestAsync(string cnp);

        Task<int> GetUserGemBalanceAsync(string cnp);

        Task UpdateUserGemBalanceAsync(string cnp, int newBalance);

        Task<string> BuyGems(string cnp, GemDeal deal, string selectedAccountId);

        Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId);
    }
}
