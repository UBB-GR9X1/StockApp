namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IUserService
    {
        Task CreateUser(User user);

        Task<User> GetUserByCnpAsync(string cnp);

        Task<User> GetCurrentUserAsync();

        string GetCurrentUserCNP();

        bool IsGuest();

        Task<List<User>> GetUsers();

        void SetCurrentUserCNP(string cnp);

        Task UpdateIsAdminAsync(bool newIsAdmin);

        Task UpdateUserAsync(string newUsername, string newImage, string newDescription, bool newHidden);
    }
}
