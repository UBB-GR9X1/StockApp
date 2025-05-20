using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Models;
using BankApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            await _repo.CreateAsync(user);
            var retrieved = await _repo.GetByCnpAsync(user.CNP);

            Assert.AreEqual(user.Username, retrieved.Username);
            Assert.AreEqual(1000, retrieved.GemBalance);
        }

        [TestMethod]
        public async Task UpdateUser_ChangesProperties()
        {
            var user = new User { CNP = "c1", Username = "Initial", GemBalance = 100 };
            await _repo.CreateAsync(user);

            user.Username = "Updated";
            user.GemBalance = 500;

            await _repo.UpdateAsync(user);
            var updated = await _repo.GetByCnpAsync("c1");

            Assert.AreEqual("Updated", updated.Username);
            Assert.AreEqual(500, updated.GemBalance);
        }

        [TestMethod]
        public async Task DeleteUser_RemovesFromRepo()
        {
            var user = new User { CNP = "c2", Username = "ToDelete", GemBalance = 100 };
            await _repo.CreateAsync(user);

            await _repo.DeleteAsync(user.Id); // Updated to pass the correct type (int)

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _repo.GetByCnpAsync("c2"));
        }
    }

    internal class InMemoryUserRepository : IUserRepository
    {
        private readonly Dictionary<string, User> _users = [];

        public string CurrentUserCNP { get; set; } = "1234567890124";

        public Task<User> CreateAsync(User user)
        {
            _users[user.CNP] = user;
            return Task.FromResult(user);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            var user = _users.Values.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User?> GetByCnpAsync(string cnp)
        {
            _users.TryGetValue(cnp, out var user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            var user = _users.Values.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<List<User>> GetAllAsync()
        {
            return Task.FromResult(_users.Values.ToList());
        }

        public Task<bool> UpdateAsync(User user)
        {
            if (!_users.ContainsKey(user.CNP))
                return Task.FromResult(false);

            _users[user.CNP] = user;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var user = _users.Values.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Task.FromResult(false);

            _users.Remove(user.CNP);
            return Task.FromResult(true);
        }
    }
}
