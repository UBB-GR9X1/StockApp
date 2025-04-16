using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockApp.Service;

namespace StockApp.ViewModel
{
    internal class ModelView
    {


        private ProfieServices profServ;

        public ModelView(string myCnp)
        {
            profServ = new ProfieServices(myCnp);
        }

        public string GetImage() => profServ.GetImage();
        public string GetUsername() => profServ.GetUsername();
        public string GetDescription() => profServ.GetDescription();
        public bool IsHidden() => profServ.IsHidden();
        public bool IsAdmin() => profServ.IsAdmin();
        public List<string> GetUserStocks() => profServ.GetUserStocks();



        public void UpdateAll(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            profServ.UpdateUser(newUsername, newImage, newDescription, newHidden);
        }

        public string GetPassword()
        {
            return profServ.GetPass();
        }

        public void UpdateAdminMode(bool newIsAdmin)
        {
            profServ.UpdateIsAdmin(newIsAdmin);
        }

    }

}

