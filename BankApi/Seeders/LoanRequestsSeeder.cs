using BankApi.Data;
using Common.Models;

namespace BankApi.Seeders
{
    public class LoanRequestsSeeder(IConfiguration configuration, IServiceProvider serviceProvider) : RegularTableSeeder<LoanRequest>(configuration, serviceProvider)
    {
        protected override async Task SeedDataAsync(ApiDbContext context)
        {
            if (context.LoanRequests.Any())
            {
                Console.WriteLine("LoanRequests already exist, skipping seeding.");
                return;
            }

            var loanRequests = new[]
            {
                new LoanRequest { UserCnp = "1234567890123", Amount = 5000.00m, ApplicationDate = new DateTime(2025, 4, 1), RepaymentDate = new DateTime(2025, 10, 1), Status = "Pending" },
                new LoanRequest { UserCnp = "9876543210987", Amount = 12000.50m, ApplicationDate = new DateTime(2025, 3, 15), RepaymentDate = new DateTime(2025, 9, 15), Status = "Approved" },
                new LoanRequest { UserCnp = "2345678901234", Amount = 3500.75m, ApplicationDate = new DateTime(2025, 2, 20), RepaymentDate = new DateTime(2025, 8, 20), Status = "Rejected" },
                new LoanRequest { UserCnp = "3456789012345", Amount = 8000.00m, ApplicationDate = new DateTime(2025, 1, 10), RepaymentDate = new DateTime(2025, 7, 10), Status = "Pending" },
                new LoanRequest { UserCnp = "4567890123456", Amount = 15000.25m, ApplicationDate = new DateTime(2025, 5, 5), RepaymentDate = new DateTime(2025, 11, 5), Status = "Approved" }
            };

            await context.LoanRequests.AddRangeAsync(loanRequests);
        }
    }
}
