namespace StockApp.Exceptions
{
    using System;

    public class AlertPersistenceException : Exception
    {
        public AlertPersistenceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class DuplicateStockException : Exception
    {
        public DuplicateStockException(string stockName)
            : base($"A stock with the name '{stockName}' already exists.") { }
    }

    public class HomepageStockPersistenceException : Exception
    {
        public HomepageStockPersistenceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class NewsPersistenceException : Exception
    {
        public NewsPersistenceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }

    public class StockPersistenceException : Exception
    {
        public StockPersistenceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
