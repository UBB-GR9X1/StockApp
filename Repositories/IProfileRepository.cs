namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IProfileRepository
    {
        User CurrentUser();

        string GenerateUsername();

        User GetUserProfile(string authorCNP);

        void UpdateRepoIsAdmin(bool isAdmin);

        void UpdateMyUser(
            string newUsername,
            string newImage,
            string newDescription,
            bool newHidden);

        List<string> UserStocks();

        string GetLoggedInUserCNP();
    }
}
