namespace StockApp.Exceptions
{
    using System;

    public class RepositoryPersistenceException : Exception
    {
        public RepositoryPersistenceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class UserRepositoryException : RepositoryPersistenceException
    {
        public UserRepositoryException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class StockRepositoryException : RepositoryPersistenceException
    {
        public StockRepositoryException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class TransactionRepositoryException : RepositoryPersistenceException
    {
        public TransactionRepositoryException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class NewsRepositoryException : RepositoryPersistenceException
    {
        public NewsRepositoryException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class AlertRepositoryException : RepositoryPersistenceException
    {
        public AlertRepositoryException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class ProfilePersistenceException : Exception
    {
        public ProfilePersistenceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
