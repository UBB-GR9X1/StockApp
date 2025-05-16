namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;

    public interface IUserRepository
    {
        public static string CurrentUserCNP { get; set; } = App.Configuration.GetSection("CurrentUserCNP").Value ?? string.Empty;

        public static bool IsGuest => string.IsNullOrEmpty(CurrentUserCNP);

        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByCnpAsync(string cnp);

        Task<User?> GetByUsernameAsync(string username);

        Task<bool> CreateAsync(User user);

        Task<bool> UpdateAsync(int id, User user);

        Task<bool> DeleteAsync(int id);
    }
}