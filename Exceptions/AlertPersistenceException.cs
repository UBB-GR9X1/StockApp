namespace StockApp.Exceptions
{
    using System;

    public class AlertPersistenceException : Exception
    {
        public AlertPersistenceException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}