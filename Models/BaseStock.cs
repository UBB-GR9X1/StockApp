namespace StockApp.Models
{
    public abstract class BaseStock : IBaseStock
    {
        public string Name { get; }

        public string Symbol { get; }

        public string AuthorCnp { get; }

        protected BaseStock(string name, string symbol, string authorCnp)
        {
            Name = name;
            Symbol = symbol;
            AuthorCnp = authorCnp;
        }
    }
}
