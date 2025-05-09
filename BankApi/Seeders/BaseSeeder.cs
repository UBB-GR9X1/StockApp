using Microsoft.Data.SqlClient;

namespace BankApi.Seeders
{
    public abstract class BaseSeeder(IConfiguration configuration)
    {
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection")!;

        protected abstract string GetQuery();

        public async Task SeedAsync()
        {
            try
            {
                using SqlConnection conn = new(connectionString);
                await conn.OpenAsync();

                using SqlCommand cmd = new(GetQuery(), conn);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Database seeding failed: {ex.Message}");
                throw;
            }
        }
    }
}