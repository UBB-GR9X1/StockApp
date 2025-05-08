using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<BaseStock> BaseStocks { get; set; }
        public DbSet<ChatReport> ChatReports { get; set; }
        public DbSet<GivenTip> GivenTips { get; set; }
        public DbSet<CreditScoreHistory> CreditScoreHistories { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<TriggeredAlert> TriggeredAlerts { get; set; }
        public DbSet<GemStore> GemStores { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<HomepageStock> HomepageStocks { get; set; } = null!;
        public DbSet<Investment> Investments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BaseStock entity
            modelBuilder.Entity<BaseStock>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<BaseStock>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<BaseStock>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<BaseStock>()
                .Property(s => s.Symbol)
                .IsRequired();

            modelBuilder.Entity<BaseStock>()
                .Property(s => s.AuthorCNP)
                .IsRequired();

            modelBuilder.Entity<HomepageStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol)
                      .IsRequired()
                      .HasMaxLength(10);
                entity.Property(e => e.CompanyName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Price)
                      .HasPrecision(18, 2); // Adjust precision and scale as needed
                entity.Property(e => e.Change)
                      .HasPrecision(18, 2); // Adjust precision and scale as needed
                entity.Property(e => e.PercentChange)
                      .HasPrecision(18, 2); // Adjust precision and scale as needed
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

            // Configure Alert entity
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasIndex(a => a.StockName);

                entity.Property(a => a.LowerBound)
                      .HasPrecision(18, 4);

                entity.Property(a => a.UpperBound)
                      .HasPrecision(18, 4);
            });

            // Configure TriggeredAlert entity
            modelBuilder.Entity<TriggeredAlert>()
                .HasIndex(ta => ta.StockName);

            // Configure ActivityLog entity
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

            modelBuilder.Entity<GemStore>(entity =>
            {
                entity.ToTable("GemStores");
                entity.HasKey(e => e.Cnp);
                entity.Property(e => e.Cnp).IsRequired().HasMaxLength(13);
                entity.Property(e => e.GemBalance).IsRequired();
                entity.Property(e => e.IsGuest).IsRequired();
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
        }
    }
}