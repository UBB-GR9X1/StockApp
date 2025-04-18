namespace StockApp.Models
{
    public class BaseStock(string name, string symbol, string authorCnp)
    {
        public string Name { get; } = name;

        public string Symbol { get; } = symbol;

        public string AuthorCNP { get; } = authorCnp;
    }
}
