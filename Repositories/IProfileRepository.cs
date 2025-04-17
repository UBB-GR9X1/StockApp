using System.Collections.Generic;
using StockApp.Models;

namespace StockApp.Repository
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IProfileRepository
    {
        IUser CurrentUser();

        string GenerateUsername();

        IUser GetUserProfile(string authorCNP);

        void UpdateRepoIsAdmin(bool isAdmin);

        void UpdateMyUser(
            string newUsername,
            string newImage,
            string newDescription,
            bool newHidden);

        IReadOnlyList<string> UserStocks();

        string GetLoggedInUserCNP();
    }
}
