namespace BankApi.Seeders
{
    using Microsoft.Data.SqlClient;

    public class HomepageStocksSeeder(IConfiguration configuration) : ForeignKeyTableSeeder(configuration)
    {
        protected override async Task<List<int>> GetForeignKeys()
        {
            List<int> stockIds = [];

            // Fetch existing Stock IDs dynamically
            using SqlConnection conn = new(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new("SELECT Id FROM BaseStocks ORDER BY Id ASC", conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                stockIds.Add(reader.GetInt32(0));
            }

            // Ensure we have at least 5 stocks before proceeding
            if (stockIds.Count < 5)
            {
                throw new InvalidOperationException("Not enough stocks in the database to seed HomepageStocks.");
            }

            return stockIds;
        }

        protected override async Task<string> GetQueryAsync()
        {
            var foreignKeys = await GetForeignKeys();

            return $@"
                IF NOT EXISTS (SELECT 1 FROM HomepageStocks) 
                BEGIN
                    INSERT INTO HomepageStocks 
                        (Id, Symbol, Change)
                    VALUES
                        ({foreignKeys[0]}, 'AAPL', 1.25),
                        ({foreignKeys[1]}, 'GOOGL', -0.75),
                        ({foreignKeys[2]}, 'TSLA', 2.30),
                        ({foreignKeys[3]}, 'AMZN', -1.10),
                        ({foreignKeys[4]}, 'MSFT', 0.50);
                END;
            ";
        }
    }
}