namespace StockApp.Services.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public void SetCurrentUserCNP(string cnp)
        {
            if (string.IsNullOrWhiteSpace(cnp))
            {
                throw new ArgumentException("CNP cannot be empty");
            }

            IUserRepository.CurrentUserCNP = cnp;
        }

        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            if (string.IsNullOrWhiteSpace(cnp))
            {
                throw new ArgumentException("CNP cannot be empty");
            }

            return await userRepository.GetByCnpAsync(cnp) ?? throw new KeyNotFoundException($"User with CNP {cnp} not found.");
        }

        public Task<List<User>> GetUsers()
        {
            return userRepository.GetAllAsync();
        }

        public async Task CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await userRepository.CreateAsync(user);
        }

        public string GetCurrentUserCNP()
        {
            return IUserRepository.CurrentUserCNP;
        }

        public bool IsGuest()
        {
            return IUserRepository.IsGuest;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var cnp = IUserRepository.CurrentUserCNP;
            if (string.IsNullOrWhiteSpace(cnp))
            {
                throw new ArgumentException("CNP cannot be empty");
            }

            return await userRepository.GetByCnpAsync(cnp);
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
            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newImage) || string.IsNullOrWhiteSpace(newDescription))
            {
                throw new ArgumentException("Username, image, and description cannot be empty");
            }

            User user = await this.GetCurrentUserAsync() ?? throw new KeyNotFoundException($"User with CNP {IUserRepository.CurrentUserCNP} not found.");
            user.Username = newUsername;
            user.Image = newImage;
            user.Description = newDescription;
            user.IsHidden = newHidden;
            await this.userRepository.UpdateAsync(user.Id, user);
        }

        /// <summary>
        /// Updates the admin status of the current user.
        /// </summary>
        /// <param name="isAdmin"> Indicates if the user should be an admin.</param>
        public async Task UpdateIsAdminAsync(bool isAdmin)
        {
            User user = await this.GetCurrentUserAsync() ?? throw new KeyNotFoundException($"User with CNP {IUserRepository.CurrentUserCNP} not found.");
            user.IsModerator = isAdmin;
            await this.userRepository.UpdateAsync(user.Id, user);
        }
    }
}