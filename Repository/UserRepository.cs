namespace StockApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    public class UserRepository
    {
        // Create a new user
        public async Task CreateUserAsync(User user)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("INSERT INTO [USER] (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) VALUES (@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)", connection);
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

        // Read a user by CNP
        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("SELECT * FROM [USER] WHERE CNP = @CNP", connection);
            command.Parameters.AddWithValue("@CNP", cnp);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
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

            throw new KeyNotFoundException($"No user found with CNP: {cnp}");
        }

        // Update a user
        public async Task UpdateUserAsync(User user)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("UPDATE [USER] SET NAME = @Name, DESCRIPTION = @Description, IS_HIDDEN = @IsHidden, IS_ADMIN = @IsAdmin, PROFILE_PICTURE = @ProfilePicture, GEM_BALANCE = @GemBalance WHERE CNP = @CNP", connection);
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

        // Delete a user
        public async Task DeleteUserAsync(string cnp)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("DELETE FROM [USER] WHERE CNP = @CNP", connection);
            command.Parameters.AddWithValue("@CNP", cnp);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // Get all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("SELECT * FROM [USER]", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
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
