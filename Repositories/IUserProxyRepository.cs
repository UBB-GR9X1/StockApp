using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories
{
    public interface IUserProxyRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByCnpAsync(string cnp);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> CreateAsync(User user);
        Task<bool> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);
    }
}
