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
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockValue> StockValues { get; set; }
        public DbSet<UserStock> UserStocks { get; set; }
        public DbSet<FavoriteStock> FavoriteStocks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StockPage> StockPages { get; set; }

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

            // Configure Stock entity
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => e.StockName);
                entity.Property(e => e.StockName).IsRequired();
                entity.Property(e => e.StockSymbol).IsRequired();
                entity.Property(e => e.AuthorCNP).IsRequired();
                
                entity.HasOne(e => e.Author)
                    .WithMany()
                    .HasForeignKey(e => e.AuthorCNP)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure StockValue entity
            modelBuilder.Entity<StockValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StockName).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();

                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.StockValues)
                    .HasForeignKey(e => e.StockName)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserStock entity
            modelBuilder.Entity<UserStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserCNP).IsRequired();
                entity.Property(e => e.StockName).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserCNP)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.UserStocks)
                    .HasForeignKey(e => e.StockName)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure FavoriteStock entity
            modelBuilder.Entity<FavoriteStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserCNP).IsRequired();
                entity.Property(e => e.StockName).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserCNP)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.FavoriteStocks)
                    .HasForeignKey(e => e.StockName)
                    .OnDelete(DeleteBehavior.Cascade);
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

            // Configure StockPage entity
            modelBuilder.Entity<StockPage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StockName).IsRequired();
                entity.Property(e => e.StockSymbol).IsRequired();
                entity.Property(e => e.AuthorCNP).IsRequired();
                entity.Property(e => e.CurrentPrice).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Author)
                    .WithMany()
                    .HasForeignKey(e => e.AuthorCNP)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Stock)
                    .WithMany()
                    .HasForeignKey(e => e.StockName)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.StockValues)
                    .WithOne()
                    .HasForeignKey(e => e.StockName)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserStocks)
                    .WithOne()
                    .HasForeignKey(e => e.StockName)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.FavoriteStocks)
                    .WithOne()
                    .HasForeignKey(e => e.StockName)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}