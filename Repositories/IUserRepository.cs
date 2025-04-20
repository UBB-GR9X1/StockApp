using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories
{
    public interface IUserRepository
    {
        string CurrentUserCnp { get; set; }

        Task CreateUserAsync(User user);

        Task DeleteUserAsync(string cnp);

        Task<List<User>> GetAllUsersAsync();

        Task<User> GetUserByCnpAsync(string cnp);

        Task UpdateUserAsync(User user);
    }
}