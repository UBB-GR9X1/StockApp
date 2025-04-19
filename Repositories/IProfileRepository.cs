using System.Collections.Generic;
using StockApp.Models;

namespace StockApp.Repositories
{
    public interface IProfileRepository
    {
        User CurrentUser();

        string GenerateUsername();

        User GetUserProfile(string authorCNP);

        void UpdateMyUser(string newUsername, string newImage, string newDescription, bool newHidden);

        void UpdateRepoIsAdmin(bool isAdmin);

        List<Stock> UserStocks();
    }
}