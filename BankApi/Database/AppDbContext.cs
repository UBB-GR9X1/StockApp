using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<GemStore> GemStores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GemStore>(entity =>
            {
                entity.ToTable("GemStore");
                entity.HasKey(e => e.Cnp);
                entity.Property(e => e.Cnp).IsRequired().HasMaxLength(13);
                entity.Property(e => e.GemBalance).IsRequired();
                entity.Property(e => e.IsGuest).IsRequired();
                entity.Property(e => e.LastUpdated).IsRequired();
            });
        }
    }
}
