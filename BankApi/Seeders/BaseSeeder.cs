using Microsoft.Data.SqlClient;

namespace BankApi.Seeders
{
    public class BaseSeeder(IConfiguration configuration, string query)
    {
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public async Task SeedAsync()
        {
            try
            {
                using SqlConnection conn = new(connectionString);
                await conn.OpenAsync();

                using SqlCommand cmd = new(query, conn);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database seeding failed: {ex.Message}");
            }
        }
    }
}