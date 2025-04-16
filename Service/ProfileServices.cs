using System.Collections.Generic;
using StockApp.Model;
using StockApp.Repository;
namespace StockApp.Service
{
    public class ProfileService
    {
        ProfileRepository _repo;

        private User _user;
        private List<string> userStocks;

        public ProfileService(string authorCnp)
        {
            _repo = new ProfileRepository(authorCnp);

            _user = _repo.CurrentUser();
            userStocks = _repo.userStocks();
        }

        public string GetImage() => _user.Image;
        public string GetUsername() => _user.Username;
        public string GetDescription() => _user.Description;
        public bool IsHidden() => _user.IsHidden;
        public bool IsAdmin() => _user.IsModerator;
        public List<string> GetUserStocks() => userStocks;
        public string GetPass() => "BombardinoCrocodilo";

        public void UpdateUser(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            _repo.updateMyUser(newUsername, newImage, newDescription, newHidden);
        }

        public void UpdateIsAdmin(bool isAdm)
        {
            _repo.updateRepoIsAdmin(isAdm);
        }

        public List<string> ExtractStockNames()
        {
            List<string> stockNames = new List<string>();

            foreach (var stockInfo in _repo.userStocks())
            {
                var parts = stockInfo.Split('|');
                if (parts.Length >= 2)
                {
                    string stockName = parts[1].Trim();
                    stockNames.Add(stockName);
                }
            }

            return stockNames;
        }

        public string ExtractStockName(string fullStockInfo)
        {
            var parts = fullStockInfo.Split('|');
            string extractedName = parts[1].Trim();
            return extractedName;
        }

        public string GetLoggedInUserCnp()
        {
            return _repo.getLoggedInUserCNP();
        }
    }
}
