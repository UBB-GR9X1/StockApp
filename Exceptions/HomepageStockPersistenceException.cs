namespace StockApp.Exceptions
{
    using System;

    public class HomepageStockPersistenceException : Exception
    {
        public HomepageStockPersistenceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
