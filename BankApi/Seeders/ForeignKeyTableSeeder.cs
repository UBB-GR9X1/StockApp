namespace BankApi.Seeders
{
    using Microsoft.Data.SqlClient;

    public abstract class ForeignKeyTableSeeder<FKType>(IConfiguration configuration, string ColumnName = "Id") : TableSeeder(configuration)
    {
        private readonly string ColumName = ColumnName;
        public override async Task SeedAsync()
        {
            try
            {
                using SqlConnection conn = new(this.connectionString);
                await conn.OpenAsync();

                using SqlCommand cmd = new(await this.GetQueryAsync(), conn);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Database seeding failed: {ex.Message}");
                throw;
            }
        }

        protected abstract string GetReferencedTableName();

        protected abstract string GetQueryWithForeignKeys(List<FKType> foreignKeys);

        private async Task<List<FKType>> GetForeignKeys()
        {
            List<FKType> fks = new();
            string referencedTableName = this.GetReferencedTableName();

            using SqlConnection conn = new(this.connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new($"SELECT {ColumName} FROM {referencedTableName} ORDER BY {ColumName} ASC", conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                if (typeof(FKType) == typeof(int))
                {
                    fks.Add((FKType)(object)reader.GetInt32(0));
                }
                else if (typeof(FKType) == typeof(string))
                {
                    fks.Add((FKType)(object)reader.GetString(0));
                }
                else
                {
                    throw new InvalidOperationException("Unsupported FKType. Only int and string are supported.");
                }
            }

            // Ensure we have at least 5 records before proceeding
            if (fks.Count < 5)
            {
                throw new InvalidOperationException($"Not enough records in the {referencedTableName} table to seed.");
            }

            return fks;
        }

        private async Task<string> GetQueryAsync()
        {
            var foreignKeys = await this.GetForeignKeys();
            return this.GetQueryWithForeignKeys(foreignKeys);
        }
    }
}
