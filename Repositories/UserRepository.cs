namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using StockApp.Database;
    using StockApp.Exceptions;
    using StockApp.Models;

    public class UserRepository(string? cnp = null) : IUserRepository
    {
        /// <summary>
        /// Gets or sets the CNP (unique identifier) of the current user.
        /// </summary>
        public string CurrentUserCNP { get; set; } = !string.IsNullOrWhiteSpace(cnp)
                ? cnp
                : App.Configuration["DefaultUserCNP"]
                    ?? throw new InvalidOperationException("DefaultUserCNP is not set in appsettings.json");

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
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
                await connection.CloseAsync();
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to create user in the database.", ex);
            }
        }

        /// <summary>
        /// Retrieves a user by their CNP.
        /// </summary>
        /// <param name="userCNP">The CNP of the user to retrieve.</param>
        /// <returns>The user matching the specified CNP.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no user with the specified CNP exists.</exception>
        /// <exception cref="UserRepositoryException">Thrown when there is an error retrieving the user.</exception>
        public async Task<User> GetUserByCnpAsync(string userCNP)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var command = new SqlCommand("SELECT * FROM [USER] WHERE CNP = @cnp", connection);
                command.Parameters.AddWithValue("@cnp", userCNP);

                using var reader = command.ExecuteReader();
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

                throw new KeyNotFoundException($"No user found with CNP: {userCNP}");
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to retrieve user from the database.", ex);
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var command = new SqlCommand("SELECT * FROM [USER] WHERE NAME = @name", connection);
                command.Parameters.AddWithValue("@name", username);
                using var reader = command.ExecuteReader();
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
                throw new KeyNotFoundException($"No user found with username: {username}");
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to retrieve user from the database.", ex);
            }
        }


        /// <summary>
        /// Updates an existing user's details in the database.
        /// </summary>
        /// <param name="user">The user entity with updated information.</param>
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
                await connection.CloseAsync();
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to update user in the database.", ex);
            }
        }

        /// <summary>
        /// Deletes a user from the database by their CNP.
        /// </summary>
        /// <param name="userCNP">The CNP of the user to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserRepositoryException">Thrown when there is an error deleting the user.</exception>
        public async Task DeleteUserAsync(string userCNP)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var command = new SqlCommand("DELETE FROM [USER] WHERE CNP = @cnp", connection);
                command.Parameters.AddWithValue("@cnp", userCNP);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to delete user from the database.", ex);
            }
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A list of all user entities.</returns>
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

                await connection.CloseAsync();
                return users;
            }
            catch (SqlException ex)
            {
                throw new UserRepositoryException("Failed to retrieve users from the database.", ex);
            }
        }
    }
}
