using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Profile
{
    internal class UpdateProfilePageViewModel
    {


        private ProfieServices profServ = ProfieServices.Instance;

        public UpdateProfilePageViewModel()
        {

        }

        public string getImage() => profServ.getImage();
        public string getUsername() => profServ.getUsername();
        public string getDescription() => profServ.getDescription();
        public bool isHidden() => profServ.isHidden();
        public bool isAdmin() => profServ.isAdmin();
        public List<string> getUserStocks() => profServ.getUserStocks();

        public void updateAll(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            profServ.updateUser(newUsername, newImage, newDescription, newHidden);
        }

        public string getPassword()
        {
            return profServ.getPass();
        }

        public void updateAdminMode(bool newIsAdmin)
        {
            profServ.updateIsAdmin(newIsAdmin);
        }

    }

}

