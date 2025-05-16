using BankApi.Data;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApiDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByCnpAsync(string cnp)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.CNP == cnp);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
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
