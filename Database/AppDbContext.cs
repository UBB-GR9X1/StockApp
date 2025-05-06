using Microsoft.EntityFrameworkCore;
using StockApp.Models;

namespace StockApp.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BaseStock> BaseStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BaseStock entity
            modelBuilder.Entity<BaseStock>(entity =>
            {
                entity.ToTable("STOCK");
                entity.HasKey(e => e.Name);
                entity.Property(e => e.Name).HasColumnName("STOCK_NAME");
                entity.Property(e => e.Symbol).HasColumnName("STOCK_SYMBOL");
                entity.Property(e => e.AuthorCNP).HasColumnName("AUTHOR_CNP");
            });
        }
    }
} 