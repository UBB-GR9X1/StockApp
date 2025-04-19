namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Gets or sets the CNP (unique identifier) of the current user.
        /// </summary>
        public string CurrentUserCNP { get; set; } = "1234567890124";

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateUserAsync(User user)
        {
            // Prepare database connection and insertion command
            using var connection = DatabaseHelper.GetConnection();
            // FIXME: Consider wrapping SqlCommand in a using statement to ensure proper disposal.
            var command = new SqlCommand(
                "INSERT INTO [USER] (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) " +
                "VALUES (@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)",
                connection);

            // Set parameters (use DBNull for optional fields)
            command.Parameters.AddWithValue("@CNP", user.CNP);
            command.Parameters.AddWithValue("@Name", user.Username);
            command.Parameters.AddWithValue("@Description", user.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsHidden", user.IsHidden);
            command.Parameters.AddWithValue("@IsAdmin", user.IsModerator);
            command.Parameters.AddWithValue("@ProfilePicture", user.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@GemBalance", user.GemBalance);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Retrieves a user by their CNP.
        /// </summary>
        /// <param name="cnp">The CNP of the user to retrieve.</param>
        /// <returns>The user matching the specified CNP.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no user with the specified CNP exists.</exception>
        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("SELECT * FROM [USER] WHERE CNP = @CNP", connection);
            command.Parameters.AddWithValue("@CNP", cnp);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                // Map database columns to User model
                return new User
                {
                    CNP = reader["CNP"].ToString(),
                    Username = reader["NAME"].ToString(),
                    Description = reader["DESCRIPTION"] as string,
                    IsHidden = (bool)reader["IS_HIDDEN"],
                    IsModerator = (bool)reader["IS_ADMIN"],
                    Image = reader["PROFILE_PICTURE"] as string,
                    GemBalance = (int)reader["GEM_BALANCE"],
                };
            }

            throw new KeyNotFoundException($"User with CNP '{cnp}' not found.");
        }

        /// <summary>
        /// Updates an existing user's details in the database.
        /// </summary>
        /// <param name="user">The user entity with updated information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateUserAsync(User user)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand(
                "UPDATE [USER] SET NAME = @Name, DESCRIPTION = @Description, IS_HIDDEN = @IsHidden, " +
                "IS_ADMIN = @IsAdmin, PROFILE_PICTURE = @ProfilePicture, GEM_BALANCE = @GemBalance " +
                "WHERE CNP = @CNP",
                connection);

            command.Parameters.AddWithValue("@CNP", user.CNP);
            command.Parameters.AddWithValue("@Name", user.Username);
            command.Parameters.AddWithValue("@Description", user.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsHidden", user.IsHidden);
            command.Parameters.AddWithValue("@IsAdmin", user.IsModerator);
            command.Parameters.AddWithValue("@ProfilePicture", user.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@GemBalance", user.GemBalance);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes a user from the database by their CNP.
        /// </summary>
        /// <param name="cnp">The CNP of the user to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteUserAsync(string cnp)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("DELETE FROM [USER] WHERE CNP = @CNP", connection);
            command.Parameters.AddWithValue("@CNP", cnp);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A list of all user entities.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("SELECT * FROM [USER]", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Map each database record to a User model
                users.Add(new User
                {
                    CNP = reader["CNP"].ToString() ?? throw new Exception("CNP not found."),
                    Username = reader["NAME"].ToString() ?? throw new Exception("Username not found."),
                    Description = reader["DESCRIPTION"] as string ?? string.Empty,
                    IsHidden = Convert.ToBoolean(reader["IS_HIDDEN"]),
                    IsModerator = Convert.ToBoolean(reader["IS_ADMIN"]),
                    Image = reader["PROFILE_PICTURE"].ToString() ?? string.Empty,
                    GemBalance = Convert.ToInt32(reader["GEM_BALANCE"]),
                });
            }

            return users;
        }
    }
}
