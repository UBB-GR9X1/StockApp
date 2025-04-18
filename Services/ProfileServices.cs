namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;
    using StockApp.Repositories;

    public class ProfieServices : IProfileService
    {
        ProfileRepository _repo;

        private User _user;
        private List<string> userStocks;

        public ProfileServices(string authorCnp)
        {
            _repo = new ProfileRepository(authorCnp);

            _user = _repo.CurrentUser();
            userStocks = _repo.UserStocks();
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
            _repo.UpdateMyUser(newUsername, newImage, newDescription, newHidden);
            /*
                        _user.Username = newUsername;
                        _user.Image = newImage;
                        _user.Description = newDescription;
                        _user.IsHidden = newHidden;*/
        }

        public void UpdateIsAdmin(bool isAdm)
        {
            //_user.IsModerator = isAdm;
            _repo.UpdateRepoIsAdmin(isAdm);
        }

        public List<string> ExtractStockNames()
        {
            List<string> stockNames = new List<string>();

            foreach (var stockInfo in _repo.UserStocks())
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

        public string ExtractStockName(string fullStockInfo)
        {
            var parts = fullStockInfo.Split('|');
            string extractedName = parts[1].Trim();
            return extractedName;

        }

        public string GetLoggedInUserCnp()
        {
            return _repo.GetLoggedInUserCNP();
        }

    }
}
