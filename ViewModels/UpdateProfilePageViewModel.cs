namespace StockApp.ViewModels
{
    using System.Collections.Generic;
    using StockApp.Services;

    internal class UpdateProfilePageViewModel
    {


        private ProfieServices profServ;

        public UpdateProfilePageViewModel(string myCnp)
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

