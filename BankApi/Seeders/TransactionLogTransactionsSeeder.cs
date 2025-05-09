namespace BankApi.Seeders
{
    using Microsoft.Data.SqlClient;

    public class TransactionLogTransactionsSeeder(IConfiguration configuration) : ForeignKeyTableSeeder(configuration)
    {
        protected override async Task<List<int>> GetForeignKeys()
        {
            List<int> userIds = [];

            // Fetch existing User IDs dynamically
            using SqlConnection conn = new(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new("SELECT Id FROM Users ORDER BY Id ASC", conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                userIds.Add(reader.GetInt32(0));
            }

            // Ensure we have at least 5 users before proceeding
            if (userIds.Count < 5)
            {
                throw new InvalidOperationException("Not enough users in the database to seed TransactionLogTransactions.");
            }

            return userIds;
        }

        protected override async Task<string> GetQueryAsync()
        {
            var foreignKeys = await GetForeignKeys();

            return $@"
                IF NOT EXISTS (SELECT 1 FROM TransactionLogTransactions) 
                BEGIN
                    INSERT INTO TransactionLogTransactions 
                        (Id, StockSymbol, StockName, Type, Amount, PricePerStock, Date)
                    VALUES
                        ({foreignKeys[0]}, 'AAPL', 'Apple Inc.', 'BUY', 10, 150, '2025-04-01'),
                        ({foreignKeys[1]}, 'GOOGL', 'Alphabet Inc.', 'SELL', 5, 2800, '2025-03-15'),
                        ({foreignKeys[2]}, 'TSLA', 'Tesla Inc.', 'BUY', 20, 750, '2025-02-20'),
                        ({foreignKeys[3]}, 'AMZN', 'Amazon.com Inc.', 'SELL', 8, 3400, '2025-01-10'),
                        ({foreignKeys[4]}, 'MSFT', 'Microsoft Corp.', 'BUY', 15, 320, '2025-05-05');
                END;
            ";
        }
    }
}