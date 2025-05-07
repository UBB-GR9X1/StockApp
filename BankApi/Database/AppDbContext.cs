namespace BankApi.Database
{
    using Microsoft.EntityFrameworkCore;
    using BankApi.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<HomepageStock> HomepageStocks { get; set; }
        //public DbSet<User> Users { get; set; } // if User was already there

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HomepageStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired();
                entity.Property(e => e.CompanyName).IsRequired();
            });
        }
    }
}
