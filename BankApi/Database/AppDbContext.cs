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

        /// <summary>
        /// Gets or sets the Tips table.
        /// </summary>
        public DbSet<Tip> Tips { get; set; }

        /// <summary>
        /// Gets or sets the GivenTips table.
        /// </summary>
        public DbSet<GivenTip> GivenTips
        {
            get; set;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HomepageStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired();
                entity.Property(e => e.CompanyName).IsRequired();


            });
            modelBuilder.Entity<Tip>(entity =>
            {
                entity.HasKey(t => t.TipId);
                entity.Property(t => t.User).IsRequired();
                entity.Property(t => t.StockName).IsRequired();
                entity.Property(t => t.Message).IsRequired();
            });

            modelBuilder.Entity<GivenTip>(entity =>
            {
                entity.HasKey(gt => gt.Id);
                entity.Property(gt => gt.TipId).IsRequired();
                entity.Property(gt => gt.GivenToUser).IsRequired();
            });
        }
    }
}
