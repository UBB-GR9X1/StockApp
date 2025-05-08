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

        public DbSet<ActivityLog> ActivityLogs { get; set; }
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
            modelBuilder.Entity<Alert>()
                .HasIndex(a => a.StockName);

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
        }
    }
}