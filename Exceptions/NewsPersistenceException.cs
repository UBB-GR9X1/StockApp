namespace StockApp.Exceptions
{
    using System;

    public class NewsPersistenceException : Exception
    {
        public NewsPersistenceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
