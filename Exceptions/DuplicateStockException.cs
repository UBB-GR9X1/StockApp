namespace StockApp.Exceptions
{
    using System;

    public class DuplicateStockException : Exception
    {
        public DuplicateStockException(string stockName)
            : base($"A stock with the name '{stockName}' already exists.") { }
    }
}
