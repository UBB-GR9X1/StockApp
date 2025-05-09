using Microsoft.Data.SqlClient;

namespace BankApi.Seeders
{
    public class ChatReportsDatabaseSeeder
    {
        private readonly string _connectionString;

        public ChatReportsDatabaseSeeder(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task InsertUsersAsync()
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                string query = @"
                    IF NOT EXISTS (SELECT 1 FROM ChatReports) 
                    BEGIN
                        INSERT INTO ChatReports 
                            (ReportedUserCnp, ReportedMessage)
                        VALUES
                            ('1234567890123', 'This user sent inappropriate content.'),
                            ('9876543210987', 'Reported for spamming multiple messages.'),
                            ('2345678901234', 'This user violated chat guidelines.'),
                            ('3456789012345', 'Reported for offensive language.'),
                            ('4567890123456', 'User harassed another member.');
                    END;
                ";

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