using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repository
{
    /// <summary>
    /// Interface for user repository operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateUserAsync(User user);

        /// <summary>
        /// Retrieves a user by their CNP.
        /// </summary>
        /// <param name="cnp">The CNP of the user to retrieve.</param>
        /// <returns>The user with the specified CNP.</returns>
        Task<User> GetUserByCnpAsync(string cnp);

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="cnp">The CNP of the user to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteUserAsync(string cnp);

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A list of all users.</returns>
        Task<List<User>> GetAllUsersAsync();
    }
}