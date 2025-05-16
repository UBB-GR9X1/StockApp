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
    }
}