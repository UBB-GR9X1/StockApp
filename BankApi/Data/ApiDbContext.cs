using BankApi.Models;
using Microsoft.EntityFrameworkCore;
//using StockApp.Models; // IMPORTANT: Added StockApp.Models because HomepageStock is from StockApp.Models

namespace BankApi.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<BaseStock> BaseStocks { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<ChatReport> ChatReports { get; set; }
        public DbSet<GivenTip> GivenTips { get; set; }
        public DbSet<CreditScoreHistory> CreditScoreHistories { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<TriggeredAlert> TriggeredAlerts { get; set; }
        public DbSet<GemStore> GemStores { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<HomepageStock> HomepageStocks { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserStock> UserStocks { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<BillSplitReport> BillSplitReports { get; set; }
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<LoanRequest> LoanRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BaseStock configuration
            modelBuilder.Entity<BaseStock>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasIndex(s => s.Name)
                      .IsUnique();

                entity.Property(s => s.Name)
                      .IsRequired();

                entity.Property(s => s.Symbol)
                      .IsRequired();

                modelBuilder.Entity<BaseStock>()
                    .Property(s => s.AuthorCNP)
                    .IsRequired();

                // User configuration
                modelBuilder.Entity<User>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.HasIndex(e => e.CNP).IsUnique();
                    entity.Property(e => e.CNP)
                          .IsRequired()
                          .HasMaxLength(13);
                    entity.Property(e => e.Username)
                          .IsRequired()
                          .HasMaxLength(50);
                    entity.Property(e => e.FirstName)
                          .IsRequired()
                          .HasMaxLength(50);
                    entity.Property(e => e.LastName)
                          .IsRequired()
                          .HasMaxLength(50);
                    entity.Property(e => e.Email)
                          .IsRequired()
                          .HasMaxLength(100);
                    entity.Property(e => e.HashedPassword)
                          .IsRequired();
                    entity.Property(e => e.Description)
                          .HasMaxLength(500);
                    entity.Property(e => e.Image)
                          .HasMaxLength(255);
                    entity.Property(e => e.PhoneNumber)
                          .HasMaxLength(20);
                    entity.Property(e => e.GemBalance)
                          .HasDefaultValue(0);
                    entity.Property(e => e.NumberOfOffenses)
                          .HasDefaultValue(0);
                    entity.Property(e => e.RiskScore)
                          .HasDefaultValue(0);
                    entity.Property(e => e.ROI)
                          .HasPrecision(18, 2)
                          .HasDefaultValue(0);
                    entity.Property(e => e.CreditScore)
                          .HasDefaultValue(0);
                    entity.Property(e => e.Birthday)
                          .IsRequired();
                    entity.Property(e => e.ZodiacSign)
                          .HasMaxLength(50);
                    entity.Property(e => e.ZodiacAttribute)
                          .HasMaxLength(50);
                    entity.Property(e => e.NumberOfBillSharesPaid)
                          .HasDefaultValue(0);
                    entity.Property(e => e.Income)
                          .HasDefaultValue(0);
                    entity.Property(e => e.Balance)
                          .HasPrecision(18, 2)
                          .HasDefaultValue(0);
                });

                // HomepageStock configuration
                modelBuilder.Entity<HomepageStock>(entity =>
                {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.Symbol)
                          .IsRequired()
                          .HasMaxLength(10);

                    entity.Property(e => e.Change)
                          .HasPrecision(18, 2);

                    entity.HasOne(e => e.StockDetails)
                          .WithOne()
                          .HasForeignKey<HomepageStock>(e => e.Id);
                });

                modelBuilder.Entity<ChatReport>(entity =>
                {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.ReportedUserCnp)
                          .IsRequired()
                          .HasMaxLength(15);

                    entity.Property(e => e.ReportedMessage)
                          .IsRequired();
                });

                // Alert configuration
                modelBuilder.Entity<Alert>(entity =>
                {
                    entity.HasIndex(a => a.StockName);

                    entity.Property(a => a.LowerBound)
                          .HasPrecision(18, 4);

                    entity.Property(a => a.UpperBound)
                          .HasPrecision(18, 4);
                });

                // TriggeredAlert configuration
                modelBuilder.Entity<TriggeredAlert>(entity =>
                {
                    entity.HasIndex(ta => ta.StockName);
                });

                // ActivityLog configuration
                modelBuilder.Entity<ActivityLog>(entity =>
                {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.UserCnp)
                        .IsRequired()
                        .HasMaxLength(13);

                    entity.Property(e => e.ActivityName)
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(e => e.LastModifiedAmount)
                        .IsRequired();

                    entity.Property(e => e.CreatedAt)
                        .IsRequired()
                        .HasDefaultValueSql("GETUTCDATE()");
                });

                // GemStore configuration
                modelBuilder.Entity<GemStore>(entity =>
                {
                    entity.ToTable("GemStores");

                    entity.HasKey(e => e.Cnp);

                    entity.Property(e => e.Cnp)
                        .IsRequired()
                        .HasMaxLength(13);

                    entity.Property(e => e.GemBalance)
                        .IsRequired();

                    entity.Property(e => e.IsGuest)
                        .IsRequired();

                    entity.Property(e => e.LastUpdated)
                        .IsRequired();
                });

                modelBuilder.Entity<Profile>(entity =>
                {
                    entity.ToTable("Profiles");
                    entity.HasKey(e => e.Cnp);
                    entity.Property(e => e.Cnp).IsRequired().HasMaxLength(13);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.ProfilePicture).HasMaxLength(500);
                    entity.Property(e => e.Description).HasMaxLength(1000);
                    entity.Property(e => e.IsHidden).IsRequired();
                    entity.Property(e => e.IsAdmin).IsRequired();
                    entity.Property(e => e.GemBalance).IsRequired();
                    entity.Property(e => e.LastUpdated).IsRequired();
                });

                modelBuilder.Entity<Investment>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.InvestorCnp).IsRequired().HasMaxLength(13);
                    entity.Property(e => e.Details).IsRequired();
                    entity.Property(e => e.AmountInvested).HasPrecision(18, 2);
                    entity.Property(e => e.AmountReturned).HasPrecision(18, 2);
                    entity.Property(e => e.InvestmentDate).IsRequired();
                });
                // Configure BillSplitReport entity
                modelBuilder.Entity<BillSplitReport>()
                    .HasKey(b => b.Id);

                modelBuilder.Entity<BillSplitReport>()
                    .Property(b => b.ReportedUserCnp)
                    .IsRequired();

                modelBuilder.Entity<BillSplitReport>()
                    .Property(b => b.ReportingUserCnp)
                    .IsRequired();

                modelBuilder.Entity<BillSplitReport>()
                    .Property(b => b.DateOfTransaction)
                    .IsRequired();

                modelBuilder.Entity<BillSplitReport>()
                    .Property(b => b.BillShare)
                    .IsRequired()
                    .HasPrecision(18, 2); // Specify precision and scale for the decimal property

                modelBuilder.Entity<UserStock>(entity =>
                {
                    entity.HasKey(e => new { e.UserCnp, e.StockName });
                    entity.Property(e => e.UserCnp).IsRequired().HasMaxLength(13);
                    entity.Property(e => e.StockName).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.Quantity).IsRequired();
                    entity.HasOne(e => e.Stock)
                          .WithMany()
                          .HasForeignKey(e => e.StockName)
                          .HasPrincipalKey(s => s.Name);
                });

                modelBuilder.Entity<CreditScoreHistory>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.UserCnp).IsRequired().HasMaxLength(13);
                    entity.Property(e => e.Date).IsRequired();
                    entity.Property(e => e.Score).IsRequired();
                });
            });

            modelBuilder.Entity<LoanRequest>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserCnp)
                    .IsRequired()
                    .HasMaxLength(13);

                entity.Property(e => e.Amount)
                    .IsRequired();

                entity.Property(e => e.ApplicationDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.RepaymentDate)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }
    }
}
