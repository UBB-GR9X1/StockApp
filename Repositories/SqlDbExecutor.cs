namespace StockApp.Repositories
{
    using System;
    using Microsoft.Data.SqlClient;

    public class SqlDbExecutor : IDbExecutor
    {
        private readonly SqlConnection _conn;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbExecutor"/> class.
        /// </summary>
        /// <param name="conn"></param>
        public SqlDbExecutor(SqlConnection conn) => _conn = conn;

        public object? ExecuteScalar(string sql, Action<SqlCommand> parameterize)
        {
            using var cmd = new SqlCommand(sql, _conn);
            parameterize?.Invoke(cmd);
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Executes a non-query SQL command.
        /// <param name="sql">The SQL command to execute.</param>
        /// <param name="parameterize">An action to parameterize the command.</param>
        public void ExecuteNonQuery(string sql, Action<SqlCommand> parameterize)
        {
            using var cmd = new SqlCommand(sql, _conn);
            parameterize?.Invoke(cmd);
            cmd.ExecuteNonQuery();
        }
    }
}
