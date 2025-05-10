namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
            this.profileRepo = new ProfileRepository(this.userRepo.CurrentUserCNP);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService"/> class.
        /// </summary>
        /// <param name="userRepo">User repository instance.</param>
        /// <param name="profileRepo">Profile repository instance.</param>
        public ProfileService(IUserRepository userRepo, IProfileRepository profileRepo)
        {
            this.userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            this.profileRepo = profileRepo ?? throw new ArgumentNullException(nameof(profileRepo));

            if (string.IsNullOrEmpty(this.userRepo.CurrentUserCNP))
            {
                throw new InvalidOperationException("No user is currently logged in.");
            }
        }

        /// <summary>
        /// Gets the image of the current user.
        /// </summary>
        /// <returns> The image URL of the current user.</returns>
        public string GetImage() => this.profileRepo.CurrentUser().Image;

        /// <summary>
        /// Gets the username of the current user.
        /// </summary>
        /// <returns> The username of the current user.</returns>
        public string GetUsername() => this.profileRepo.CurrentUser().Username;

        /// <summary>
        /// Gets the description of the current user.
        /// </summary>
        /// <returns> The description of the current user.</returns>
        public string GetDescription() => this.profileRepo.CurrentUser().Description;

        /// <summary>
        /// Checks if the current user is hidden.
        /// </summary>
        /// <returns> True if the user is hidden; otherwise, false.</returns>
        public bool IsHidden() => this.profileRepo.CurrentUser().IsHidden;

        /// <summary>
        /// Checks if the current user is an admin.
        /// </summary>
        /// <returns> True if the user is an admin; otherwise, false.</returns>
        public bool IsAdmin() => this.profileRepo.CurrentUser().IsModerator;

        /// <summary>
        /// Gets the profile of a user by their CNP.
        /// </summary>
        /// <returns> The profile of the user.</returns>
        public List<Stock> GetUserStocks() => this.profileRepo.UserStocks();

        /// <summary>
        /// Updates the current user's profile with new information.
        /// </summary>
        /// <param name="newUsername"> The new username.</param>
        /// <param name="newImage"> The new image URL.</param>
        /// <param name="newDescription"> The new description.</param>
        /// <param name="newHidden"> Indicates if the user should be hidden.</param>
        public async Task UpdateUserAsync(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            await this.profileRepo.UpdateMyUserAsync(newUsername, newImage, newDescription, newHidden);
        }

        /// <summary>
        /// Updates the admin status of the current user.
        /// </summary>
        /// <param name="isAdmin"> Indicates if the user should be an admin.</param>
        public async Task UpdateIsAdminAsync(bool isAdmin)
        {
            await this.profileRepo.UpdateRepoIsAdminAsync(isAdmin);
        }

        public async Task<List<Stock>> GetUserStocksAsync()
        {
            return await this.profileRepo.UserStocksAsync();
        }
    }
}
