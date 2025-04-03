using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Model;

namespace StockApp.Profile
{
    public class ProfieServices
    {
        private static ProfieServices _instance;
        private static readonly object _lock = new object();

        private User _user;
        private List<string> userStocks;

        private ProfieServices()
        {
            _user = new User("1234567890", "Caramel", "asdf", false, "https://static.wikia.nocookie.net/hellokitty/images/3/32/Sanrio_Characters_Keroppi_Image007.png/revision/latest/thumbnail/width/360/height/360?cb=20170405011801", false);
            userStocks = new List<string> { "Stock A", "Stock B", "Stock C", "Stock D" };
        }

        public static ProfieServices Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ProfieServices();
                    }
                    return _instance;
                }
            }
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

            _user.Username = newUsername;
            _user.Image = newImage;
            _user.Description = newDescription;
            _user.IsHidden = newHidden;
        }

        public void updateIsAdmin(bool isAdm)
        {
            _user.IsModerator = isAdm;
        }
    }
}
