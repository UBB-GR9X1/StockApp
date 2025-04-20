namespace StockApp.Repositories
{
    using System;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDbExecutor"/> class.
    /// </summary>
    /// <param name="conn"> The SQL connection to use for executing commands.</param>
    public class SqlDbExecutor(SqlConnection conn) : IDbExecutor
    {
        private readonly SqlConnection conn = conn;

        /// <summary>
        /// Executes a SQL command and returns the first column of the first row in the result set returned by the query.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterize"></param>
        /// <returns></returns>
        public object? ExecuteScalar(string sql, Action<SqlCommand> parameterize)
        {
            using var cmd = new SqlCommand(sql, this.conn);
            parameterize?.Invoke(cmd);
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Executes a non-query SQL command.
        /// </summary>
        /// <param name="sql">The SQL command to execute.</param>
        /// <param name="parameterize">An action to parameterize the command.</param>
        public void ExecuteNonQuery(string sql, Action<SqlCommand> parameterize)
        {
            using var cmd = new SqlCommand(sql, this.conn);
            parameterize?.Invoke(cmd);
            cmd.ExecuteNonQuery();
        }
    }
}
