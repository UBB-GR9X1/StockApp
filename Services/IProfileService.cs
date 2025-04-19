namespace StockApp.Services
{
    using System.Collections.Generic;

    public interface IProfileService
    {
        string GetImage();

        string GetUsername();

        string GetDescription();

        bool IsHidden();

        bool IsAdmin();

        List<string> GetUserStocks();

        string GetLoggedInUserCnp();

        void UpdateUser(
                            string newUsername,
                            string newImage,
                            string newDescription,
                            bool newHidden);

        void UpdateIsAdmin(bool isAdmin);

        string ExtractStockName(string fullStockInfo);

        List<string> ExtractStockNames();
    }
}
