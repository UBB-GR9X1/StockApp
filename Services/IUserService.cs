namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IUserService
    {
        Task CreateUser(User user);

        User GetUserByCnp(string cnp);

        Task<User> GetCurrentUserAsync();

        string GetCurrentUserCNP();

        bool IsGuest();

        Task<List<User>> GetUsers();
    }
}
