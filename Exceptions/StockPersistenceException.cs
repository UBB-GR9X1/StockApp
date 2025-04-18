namespace StockApp.Exceptions
{
    using System;

    public class StockPersistenceException : Exception
    {
        public StockPersistenceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
