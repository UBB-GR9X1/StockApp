namespace StockApp.Repositories
{
    using System;
    using Microsoft.Data.SqlClient;

    public interface IDbExecutor
    {
        object? ExecuteScalar(string sql, Action<SqlCommand> parameterize);

        void ExecuteNonQuery(string sql, Action<SqlCommand> parameterize);
    }
}
