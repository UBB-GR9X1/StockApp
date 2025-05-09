using Microsoft.Data.SqlClient;

namespace BankApi.Seeders
{
    public abstract class RegularTableSeeder(IConfiguration configuration) : TableSeeder(configuration)
    {
        protected abstract string GetQuery();

        public override async Task SeedAsync()
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
