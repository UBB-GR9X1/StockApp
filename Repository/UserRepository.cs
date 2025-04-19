namespace StockApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using StockApp.Database;
    using StockApp.Exceptions;
    using StockApp.Models;

    /// <summary>
    /// Repository class for managing user data in the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        public string CurrentUserCNP { get; private set; }

        public UserRepository(string? cnp = null)
        {
            this.CurrentUserCNP = !string.IsNullOrWhiteSpace(cnp)
                ? cnp
                : App.Configuration["DefaultUserCNP"]
                    ?? throw new InvalidOperationException("DefaultUserCNP is not set in appsettings.json");
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserRepositoryException">Thrown when there is an error creating the user.</exception>
        public async Task CreateUserAsync(User user)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var command = new SqlCommand("INSERT INTO [USER] (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) VALUES (@cnp, @name, @description, @isHidden, @isAdmin, @profilePicture, @gemBalance)", connection);
                command.Parameters.AddWithValue("@cnp", user.CNP);
                command.Parameters.AddWithValue("@name", user.Username);
                command.Parameters.AddWithValue("@description", user.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isHidden", user.IsHidden);
                command.Parameters.AddWithValue("@isAdmin", user.IsModerator);
                command.Parameters.AddWithValue("@profilePicture", user.Image ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gemBalance", user.GemBalance);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to create user in the database.", ex);
            }
        }

        /// <summary>
        /// Retrieves a user by their CNP.
        /// </summary>
        /// <param name="cnp">The CNP of the user to retrieve.</param>
        /// <returns>The user with the specified CNP.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no user is found with the specified CNP.</exception>
        /// <exception cref="UserRepositoryException">Thrown when there is an error retrieving the user.</exception>
        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var command = new SqlCommand("SELECT * FROM [USER] WHERE CNP = @cnp", connection);
                command.Parameters.AddWithValue("@cnp", cnp);

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
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to retrieve user from the database.", ex);
            }
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserRepositoryException">Thrown when there is an error updating the user.</exception>
        public async Task UpdateUserAsync(User user)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var command = new SqlCommand("UPDATE [USER] SET NAME = @name, DESCRIPTION = @description, IS_HIDDEN = @isHidden, IS_ADMIN = @isAdmin, PROFILE_PICTURE = @profilePicture, GEM_BALANCE = @gemBalance WHERE CNP = @cnp", connection);
                command.Parameters.AddWithValue("@cnp", user.CNP);
                command.Parameters.AddWithValue("@name", user.Username);
                command.Parameters.AddWithValue("@description", user.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isHidden", user.IsHidden);
                command.Parameters.AddWithValue("@isAdmin", user.IsModerator);
                command.Parameters.AddWithValue("@profilePicture", user.Image ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gemBalance", user.GemBalance);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to update user in the database.", ex);
            }
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="cnp">The CNP of the user to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserRepositoryException">Thrown when there is an error deleting the user.</exception>
        public async Task DeleteUserAsync(string cnp)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var command = new SqlCommand("DELETE FROM [USER] WHERE CNP = @cnp", connection);
                command.Parameters.AddWithValue("@cnp", cnp);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to delete user from the database.", ex);
            }
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A list of all users.</returns>
        /// <exception cref="UserRepositoryException">Thrown when there is an error retrieving the users.</exception>
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
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
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to retrieve users from the database.", ex);
            }
        }
    }
}
