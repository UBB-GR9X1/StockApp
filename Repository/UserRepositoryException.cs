using System;

namespace StockApp.Repository
{
    /// <summary>
    /// Exception thrown when an error occurs in the UserRepository.
    /// </summary>
    public class UserRepositoryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepositoryException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UserRepositoryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepositoryException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UserRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
} 