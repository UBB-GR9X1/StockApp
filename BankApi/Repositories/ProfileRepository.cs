using BankApi.Data;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApiDbContext _context;

        public ProfileRepository(ApiDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Profile?> GetProfileByCnpAsync(string cnp)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.Cnp == cnp);
        }

        public async Task<Profile> CreateProfileAsync(Profile profile)
        {
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            var existingProfile = await _context.Profiles.FindAsync(profile.Cnp);
            if (existingProfile == null)
            {
                throw new KeyNotFoundException($"Profile with CNP {profile.Cnp} not found.");
            }

            existingProfile.Name = profile.Name;
            existingProfile.ProfilePicture = profile.ProfilePicture;
            existingProfile.Description = profile.Description;
            existingProfile.IsHidden = profile.IsHidden;
            existingProfile.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingProfile;
        }

        public async Task<bool> UpdateAdminStatusAsync(string cnp, bool isAdmin)
        {
            var profile = await _context.Profiles.FindAsync(cnp);
            if (profile == null)
            {
                return false;
            }

            profile.IsAdmin = isAdmin;
            profile.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Stock>> GetUserStocksAsync(string cnp)
        {
            return await _context.UserStocks
                .Where(us => us.UserCnp == cnp)
                .Include(us => us.Stock)
                .Select(us => us.Stock)
                .ToListAsync();
        }

        public string GenerateRandomUsername()
        {
            List<string> randomUsernames = new()
            {
                "macaroane_cu_branza", "ecler_cu_fistic", "franzela_", "username1",
                "snitel_cu_piure", "ceai_de_musetel", "vita_de_vie", "paine_cu_pateu",
                "floare_de_tei", "cirese_si_visine", "inghetata_roz", "tort_de_afine",
                "paste_carbonara", "amandina", "orez_cu_lapte"
            };

            Random random = new();
            return randomUsernames[random.Next(randomUsernames.Count)];
        }
    }
} 