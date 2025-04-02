using Microsoft.EntityFrameworkCore;
using Alerts.Models; // Ensure Models namespace is included

namespace Alerts.Data // Use the correct namespace
{
    public class StocksAppContext : DbContext
    {
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=DESKTOP-4O0PI57\SQLEXPRESS;Database=StocksAppDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alert>().ToTable("Alerts");
        }
    }
}
