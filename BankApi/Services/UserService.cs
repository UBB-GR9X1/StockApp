namespace BankApi.Services
{
    using BankApi.Repositories;
    using Common.Models;
    using Common.Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            return string.IsNullOrWhiteSpace(cnp)
                ? throw new ArgumentException("CNP cannot be empty")
                : await userRepository.GetByCnpAsync(cnp) ?? throw new KeyNotFoundException($"User with CNP {cnp} not found.");
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

        /// <summary>
        /// Updates the current user's profile with new information.
        /// </summary>
        /// <param name="newUsername"> The new username.</param>
        /// <param name="newImage"> The new image URL.</param>
        /// <param name="newDescription"> The new description.</param>
        /// <param name="newHidden"> Indicates if the user should be hidden.</param>
        public async Task UpdateUserAsync(string newUsername, string newImage, string newDescription, bool newHidden, string userCNP)
        {
            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newImage) || string.IsNullOrWhiteSpace(newDescription))
            {
                throw new ArgumentException("Username, image, and description cannot be empty");
            }

            User user = await this.GetUserByCnpAsync(userCNP) ?? throw new KeyNotFoundException($"User with CNP {userCNP} not found.");
            user.UserName = newUsername;
            user.Image = newImage;
            user.Description = newDescription;
            user.IsHidden = newHidden;
            await userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Updates the admin status of the current user.
        /// </summary>
        /// <param name="isAdmin"> Indicates if the user should be an admin.</param>
        public async Task UpdateIsAdminAsync(bool isAdmin, string userCNP)
        {
            User user = await this.GetUserByCnpAsync(userCNP) ?? throw new KeyNotFoundException($"User with CNP {userCNP} not found.");
            user.IsModerator = isAdmin;
            await userRepository.UpdateAsync(user);
        }

        public async Task<User> GetCurrentUserAsync(string userCNP)
        {
            return string.IsNullOrWhiteSpace(userCNP)
                ? throw new ArgumentException("CNP cannot be empty")
                : await this.GetUserByCnpAsync(userCNP) ?? throw new KeyNotFoundException($"User with CNP {userCNP} not found.");
        }

        public async Task<int> GetCurrentUserGemsAsync(string userCNP)
        {
            if (string.IsNullOrWhiteSpace(userCNP))
            {
                throw new ArgumentException("CNP cannot be empty");
            }
            User user = await this.GetUserByCnpAsync(userCNP) ?? throw new KeyNotFoundException($"User with CNP {userCNP} not found.");
            return user.GemBalance;
        }
    }
}