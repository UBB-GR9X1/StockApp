using System.Collections.Generic;
using StockApp.Model;
using StockApp.Repository;
namespace StockApp.Service
{
    public class ProfieServices
    {
        ProfileRepository _repo;

        private User _user;
        private List<string> userStocks;

        public ProfieServices(string authorCNP)
        {
            _repo = new ProfileRepository(authorCNP);

            _user = _repo.CurrentUser();
            userStocks = _repo.userStocks();
        }

        public string getImage() => _user.Image;
        public string getUsername() => _user.Username;
        public string getDescription() => _user.Description;
        public bool isHidden() => _user.IsHidden;
        public bool isAdmin() => _user.IsModerator;
        public List<string> getUserStocks() => userStocks;
        public string getPass() => "BombardinoCrocodilo";

        public void updateUser(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            _repo.updateMyUser(newUsername, newImage, newDescription, newHidden);
            /*
                        _user.Username = newUsername;
                        _user.Image = newImage;
                        _user.Description = newDescription;
                        _user.IsHidden = newHidden;*/
        }

        public void updateIsAdmin(bool isAdm)
        {
            //_user.IsModerator = isAdm;
            _repo.updateRepoIsAdmin(isAdm);
        }

        public List<string> ExtractStockNames()
        {
            List<string> stockNames = new List<string>();

            foreach (var stockInfo in _repo.userStocks())
            {
                // Assuming format: SYMBOL | NAME | Quantity: X | Price: Y
                var parts = stockInfo.Split('|');
                if (parts.Length >= 2)
                {
                    string stockName = parts[1].Trim();
                    stockNames.Add(stockName);
                }
            }

            return stockNames;
        }

        public string extractStockName(string fullStockInfo)
        {
            var parts = fullStockInfo.Split('|');
            string extractedName = parts[1].Trim();
            return extractedName;

        }

        public string getLoggedInUserCNP()
        {
            return _repo.getLoggedInUserCNP();
        }

    }
}
