namespace StockApp.Repositories
{
    using System;
    using Microsoft.Data.SqlClient;

    public class SqlDbExecutor : IDbExecutor
    {
        private readonly SqlConnection _conn;

        public SqlDbExecutor(SqlConnection conn) => _conn = conn;

        public object? ExecuteScalar(string sql, Action<SqlCommand> parameterize)
        {
            using var cmd = new SqlCommand(sql, _conn);
            parameterize?.Invoke(cmd);
            return cmd.ExecuteScalar();
        }

        public void ExecuteNonQuery(string sql, Action<SqlCommand> parameterize)
        {
            using var cmd = new SqlCommand(sql, _conn);
            parameterize?.Invoke(cmd);
            cmd.ExecuteNonQuery();
        }
    }
}
