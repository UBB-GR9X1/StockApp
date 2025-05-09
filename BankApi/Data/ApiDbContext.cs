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

                entity.Property(s => s.AuthorCNP)
                      .IsRequired();
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
        }
    }
}
