namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository profileRepo;
        private readonly IUserRepository userRepo;

        public Task<User> CurrentUserProfile => this.profileRepo.GetUserProfileAsync(this.userRepo.CurrentUserCNP);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService"/> class.
        /// </summary>
        /// <param name="userRepo">User repository instance.</param>
        /// <param name="profileRepo">Profile repository instance.</param>
        public ProfileService(IUserRepository userRepo, IProfileRepository profileRepo)
        {
            this.userRepo = userRepo;
            this.profileRepo = profileRepo;
        }

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
