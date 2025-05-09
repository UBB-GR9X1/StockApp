using BankApi.Models;

namespace BankApi.Repositories
{
    public interface IProfileRepository
    {
        Task<User?> GetProfileByCnpAsync(string cnp);
        Task<User> CreateProfileAsync(User profile);
        Task<User> UpdateProfileAsync(User profile);
        Task<bool> UpdateAdminStatusAsync(string cnp, bool isAdmin);
        Task<List<Stock>> GetUserStocksAsync(string cnp);
        string GenerateRandomUsername();
    }
}