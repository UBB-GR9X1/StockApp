namespace BankApi.Seeders
{
    using Microsoft.Data.SqlClient;

    public class GivenTipsSeeder(IConfiguration configuration) : ForeignKeyTableSeeder(configuration)
    {
        protected override async Task<string> GetQueryAsync()
        {
            List<int> tipIds = [];

            // Fetch existing Tip IDs dynamically
            using SqlConnection conn = new(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new("SELECT Id FROM Tips ORDER BY Id ASC", conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                tipIds.Add(reader.GetInt32(0));
            }

            // Ensure we have at least 5 tips before proceeding
            if (tipIds.Count < 5)
                throw new InvalidOperationException("Not enough tips in the database to seed GivenTips.");

            return $@"
                IF NOT EXISTS (SELECT 1 FROM GivenTips) 
                BEGIN
                    INSERT INTO GivenTips 
                        (TipId, UserCNP, Date)
                    VALUES
                        ({tipIds[0]}, '1234567890123', '2025-04-01'),
                        ({tipIds[1]}, '9876543210987', '2025-03-15'),
                        ({tipIds[2]}, '2345678901234', '2025-02-20'),
                        ({tipIds[3]}, '3456789012345', '2025-01-10'),
                        ({tipIds[4]}, '4567890123456', '2025-05-05');
                END;
            ";
        }
    }
}