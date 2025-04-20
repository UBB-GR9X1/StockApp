using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private IUserRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new InMemoryUserRepository();
        }

        [TestMethod]
        public async Task CreateUser_AddsUser()
        {
            var user = new User
            {
                CNP = "1234567890000",
                Username = "TestUser",
                Description = "Test",
                IsHidden = false,
                IsModerator = false,
                Image = null,
                GemBalance = 1000
            };

            await _repo.CreateUserAsync(user);
            var retrieved = await _repo.GetUserByCnpAsync(user.CNP);

            Assert.AreEqual(user.Username, retrieved.Username);
            Assert.AreEqual(1000, retrieved.GemBalance);
        }

        [TestMethod]
        public async Task UpdateUser_ChangesProperties()
        {
            var user = new User { CNP = "c1", Username = "Initial", GemBalance = 100 };
            await _repo.CreateUserAsync(user);

            user.Username = "Updated";
            user.GemBalance = 500;

            await _repo.UpdateUserAsync(user);
            var updated = await _repo.GetUserByCnpAsync("c1");

            Assert.AreEqual("Updated", updated.Username);
            Assert.AreEqual(500, updated.GemBalance);
        }

        [TestMethod]
        public async Task DeleteUser_RemovesFromRepo()
        {
            var user = new User { CNP = "c2", Username = "ToDelete", GemBalance = 100 };
            await _repo.CreateUserAsync(user);

            await _repo.DeleteUserAsync("c2");

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _repo.GetUserByCnpAsync("c2"));
        }
    }

    internal class InMemoryUserRepository : IUserRepository
    {
        private readonly Dictionary<string, User> _users = [];

        public string CurrentUserCNP { get; set; } = "1234567890124";

        public Task CreateUserAsync(User user)
        {
            _users[user.CNP] = user;
            return Task.CompletedTask;
        }

        public Task<User> GetUserByCnpAsync(string cnp)
        {
            if (_users.TryGetValue(cnp, out var user))
                return Task.FromResult(user);

            throw new KeyNotFoundException($"User with CNP '{cnp}' not found.");
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            foreach (var user in _users.Values)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult(user);
            }
            throw new KeyNotFoundException($"User with username '{username}' not found.");
        }

        public Task<List<User>> GetAllUsersAsync() =>
            Task.FromResult(new List<User>(_users.Values));

        public Task UpdateUserAsync(User user)
        {
            if (!_users.ContainsKey(user.CNP))
                throw new KeyNotFoundException();

            _users[user.CNP] = user;
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(string cnp)
        {
            _users.Remove(cnp);
            return Task.CompletedTask;
        }
    }
}
