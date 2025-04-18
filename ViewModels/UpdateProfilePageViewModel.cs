namespace StockApp.ViewModels
{
    using System.Collections.Generic;
    using StockApp.Models;
    using StockApp.Services;

    internal class UpdateProfilePageViewModel
    {


        private ProfieService profileService;

        public UpdateProfilePageViewModel()
        {
            this.profileService = new ProfieService();
        }

        public string GetImage() => this.profileService.GetImage();

        public string GetUsername() => this.profileService.GetUsername();

        public string GetDescription() => this.profileService.GetDescription();

        public bool IsHidden() => this.profileService.IsHidden();

        public bool IsAdmin() => this.profileService.IsAdmin();

        public List<Stock> GetUserStocks() => this.profileService.GetUserStocks();



        public void UpdateAll(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            this.profileService.UpdateUser(newUsername, newImage, newDescription, newHidden);
        }

        public void UpdateAdminMode(bool newIsAdmin)
        {
            this.profileService.UpdateIsAdmin(newIsAdmin);
        }

    }

}

