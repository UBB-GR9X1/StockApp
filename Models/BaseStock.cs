namespace StockApp.Models
{
    public class BaseStock
    {
        public string Name { get; }

        public string Symbol { get; }

        public string AuthorCnp { get; }

        public BaseStock(string name, string symbol, string authorCnp)
        {
            Name = name;
            Symbol = symbol;
            AuthorCnp = authorCnp;
        }
    }
}
