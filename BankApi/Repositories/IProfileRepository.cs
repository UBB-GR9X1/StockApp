using BankApi.Models;

namespace BankApi.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile?> GetProfileByCnpAsync(string cnp);
        Task<Profile> CreateProfileAsync(Profile profile);
        Task<Profile> UpdateProfileAsync(Profile profile);
        Task<bool> UpdateAdminStatusAsync(string cnp, bool isAdmin);
        Task<List<Stock>> GetUserStocksAsync(string cnp);
        string GenerateRandomUsername();
    }
} 