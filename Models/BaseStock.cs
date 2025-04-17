namespace StockApp.Models
{
    public class BaseStock(string name, string symbol, string author_cnp)
    {
        public string Name { get; set; } = name;

        public string Symbol { get; set; } = symbol;

        public string AuthorCNP { get; set; } = author_cnp;
    }
}
