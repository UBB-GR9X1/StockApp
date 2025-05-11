namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IProfileRepository
    {
        Task<string> GenerateUsernameAsync();

        Task UpdateMyUserAsync(string newUsername, string newImage, string newDescription, bool newHidden);

        Task UpdateRepoIsAdminAsync(bool isAdmin);

        Task<List<Stock>> UserStocksAsync();
    }
}