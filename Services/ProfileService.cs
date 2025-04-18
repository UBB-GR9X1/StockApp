namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;
    using StockApp.Repositories;

    public class ProfileService : IProfileService
    {
        private ProfileRepository profileRepo;
        private UserRepository userRepo;

        public ProfileService()
        {
            this.userRepo = new();
            this.profileRepo = new ProfileRepository(this.userRepo.CurrentUserCNP);
        }

        public string GetImage() => this.profileRepo.CurrentUser().Image;

        public string GetUsername() => this.profileRepo.CurrentUser().Username;

        public string GetDescription() => this.profileRepo.CurrentUser().Description;

        public bool IsHidden() => this.profileRepo.CurrentUser().IsHidden;

        public bool IsAdmin() => this.profileRepo.CurrentUser().IsModerator;

        public List<Stock> GetUserStocks() => this.profileRepo.UserStocks();

        public void UpdateUser(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            this.profileRepo.UpdateMyUser(newUsername, newImage, newDescription, newHidden);
        }

        public void UpdateIsAdmin(bool isAdmin)
        {
            this.profileRepo.UpdateRepoIsAdmin(isAdmin);
        }

        public string GetLoggedInUserCnp() => this.profileRepo.CurrentUser().CNP;
    }
}
