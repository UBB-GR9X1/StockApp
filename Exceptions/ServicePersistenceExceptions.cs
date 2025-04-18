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
    public class ExportFormatNotSupportedException : Exception
    {
        public ExportFormatNotSupportedException(string format)
            : base($"Export format '{format}' is not supported.") { }
    }
    public class InvalidSortTypeException : Exception
    {
        public InvalidSortTypeException(string sortType)
            : base($"The sort type '{sortType}' is not supported.")
        {
        }
    }
}
