namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using Src.Repos;
    using StockApp.Models;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public User GetUserByCnp(string cnp)
        {
            if (string.IsNullOrWhiteSpace(cnp))
            {
                throw new ArgumentException("CNP cannot be empty");
            }
            return userRepository.GetUserByCnp(cnp);
        }
        public List<User> GetUsers()
        {
            return userRepository.GetUsers();
        }
    }
}