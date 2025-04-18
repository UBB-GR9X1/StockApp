namespace StockApp.Exceptions
{
    using System;

    public class GuestUserOperationException : Exception
    {
        public GuestUserOperationException(string message) : base(message) { }
    }

    public class GemTransactionFailedException : Exception
    {
        public GemTransactionFailedException(string message) : base(message) { }
    }

    public class InsufficientGemsException : Exception
    {
        public InsufficientGemsException(string message) : base(message) { }
    }
}
