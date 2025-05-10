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

        public async Task<User?> GetProfileByCnpAsync(string cnp)
        {
            return await _context.Users
                .FirstOrDefaultAsync(p => p.CNP == cnp);
        }

        public async Task<User> CreateProfileAsync(User profile)
        {
            _context.Users.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<User> UpdateProfileAsync(User profile)
        {
            var existingProfile = await _context.Users.FindAsync(profile.CNP);
            if (existingProfile == null)
            {
                throw new KeyNotFoundException($"Profile with CNP {profile.CNP} not found.");
            }
            existingProfile.Description = profile.Description;
            existingProfile.Image = profile.Image;
            existingProfile.IsHidden = profile.IsHidden;
            existingProfile.GemBalance = profile.GemBalance;
            existingProfile.NumberOfOffenses = profile.NumberOfOffenses;
            existingProfile.Username = profile.Username;
            existingProfile.FirstName = profile.FirstName;
            existingProfile.LastName = profile.LastName;
            existingProfile.Email = profile.Email;
            existingProfile.PhoneNumber = profile.PhoneNumber;
            existingProfile.HashedPassword = profile.HashedPassword;
            existingProfile.RiskScore = profile.RiskScore;
            existingProfile.ROI = profile.ROI;
            existingProfile.CreditScore = profile.CreditScore;
            existingProfile.Birthday = profile.Birthday;
            existingProfile.ZodiacSign = profile.ZodiacSign;
            existingProfile.ZodiacAttribute = profile.ZodiacAttribute;
            existingProfile.NumberOfBillSharesPaid = profile.NumberOfBillSharesPaid;
            existingProfile.Income = profile.Income;
            existingProfile.Balance = profile.Balance;

            await _context.SaveChangesAsync();
            return existingProfile;
        }

        public async Task<bool> UpdateAdminStatusAsync(string cnp, bool isAdmin)
        {
            var profile = await _context.Users.FindAsync(cnp);
            if (profile == null)
            {
                return false;
            }

            profile.IsModerator = isAdmin;
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