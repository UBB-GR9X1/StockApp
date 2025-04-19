namespace StockApp.Exceptions
{
    using System;

    public class AlertPersistenceException(string message, Exception? innerException = null) : Exception(message, innerException)
    {
    }

    public class DuplicateStockException(string stockName) : Exception($"A stock with the name '{stockName}' already exists.")
    {
    }

    public class HomepageStockPersistenceException(string message, Exception? innerException = null) : Exception(message, innerException)
    {
    }

    public class NewsPersistenceException(string message, Exception? innerException = null) : Exception(message, innerException)
    {
    }

    public class StockPersistenceException(string message, Exception innerException) : Exception(message, innerException)
    {
    }

    public class ExportFormatNotSupportedException(string format) : Exception($"Export format '{format}' is not supported.")
    {
    }

    public class InvalidSortTypeException(string sortType) : Exception($"The sort type '{sortType}' is not supported.")
    {
    }
}
