using BankApi.Data;
using Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories.Impl
{
    public class UserRepository(ApiDbContext context, ILogger<UserRepository> logger, UserManager<User> userManager) : IUserRepository
    {
        private readonly ApiDbContext _context = context;
        private readonly ILogger<UserRepository> _logger = logger;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<List<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<User> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

        public async Task<User> GetByCnpAsync(string cnp) => await _context.Users.FirstOrDefaultAsync(u => u.CNP == cnp);

        public async Task<User> GetByUsernameAsync(string username) => await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        public async Task<User> CreateAsync(User user)
        {
            if (await _userManager.FindByNameAsync(user.UserName) == null && await _context.Users.AllAsync(u => u.CNP != user.CNP))
            {
                var result = await _userManager.CreateAsync(user, user.PasswordHash);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                else
                {
                    await _context.SaveChangesAsync();
                    return user;
                }
            }
            throw new InvalidOperationException($"User with username {user.UserName} or CNP {user.CNP} already exists.");
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
