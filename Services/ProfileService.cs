namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;
    using StockApp.Repositories;

    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository profileRepo;
        private readonly IUserRepository userRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService"/> class.
        /// </summary>
        public ProfileService()
        {
            this.userRepo = new UserRepository();
            this.profileRepo = new ProfileRepository(this.userRepo.CurrentUserCnp);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService"/> class.
        /// </summary>
        /// <param name="userRepo"></param>
        /// <param name="profileRepo"></param>
        public ProfileService(IUserRepository userRepo, IProfileRepository profileRepo)
        {
            this.userRepo = userRepo;
            this.profileRepo = profileRepo;
        }

        /// <summary>
        /// Gets the image of the current user.
        /// </summary>
        /// <returns></returns>
        public string GetImage() => this.profileRepo.CurrentUser().Image;

        /// <summary>
        /// Gets the username of the current user.
        /// </summary>
        /// <returns></returns>
        public string GetUsername() => this.profileRepo.CurrentUser().Username;

        /// <summary>
        /// Gets the description of the current user.
        /// </summary>
        /// <returns></returns>
        public string GetDescription() => this.profileRepo.CurrentUser().Description;

        /// <summary>
        /// Checks if the current user is hidden.
        /// </summary>
        /// <returns></returns>
        public bool IsHidden() => this.profileRepo.CurrentUser().IsHidden;

        /// <summary>
        /// Checks if the current user is an admin.
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin() => this.profileRepo.CurrentUser().IsModerator;

        /// <summary>
        /// Gets the profile of a user by their CNP.
        /// </summary>
        /// <returns></returns>
        public List<Stock> GetUserStocks() => this.profileRepo.UserStocks();

        /// <summary>
        /// Updates the current user's profile with new information.
        /// </summary>
        /// <param name="newUsername"></param>
        /// <param name="newImage"></param>
        /// <param name="newDescription"></param>
        /// <param name="newHidden"></param>
        public void UpdateUser(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            this.profileRepo.UpdateMyUser(newUsername, newImage, newDescription, newHidden);
        }

        /// <summary>
        /// Updates the admin status of the current user.
        /// </summary>
        /// <param name="isAdmin"></param>
        public void UpdateIsAdmin(bool isAdmin)
        {
            this.profileRepo.UpdateRepoIsAdmin(isAdmin);
        }

        /// <summary>
        /// Gets the CNP of the logged-in user.
        /// </summary>
        /// <returns></returns>
        public string GetLoggedInUserCnp() => this.profileRepo.CurrentUser().CNP;
    }
}
