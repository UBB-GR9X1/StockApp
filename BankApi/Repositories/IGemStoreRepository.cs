using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public interface IGemStoreRepository
    {
        Task<string> GetCnpAsync();
        Task<int> GetUserGemBalanceAsync(string cnp);
        Task UpdateUserGemBalanceAsync(string cnp, int newBalance);
        Task<bool> IsGuestAsync(string cnp);
    }
} 