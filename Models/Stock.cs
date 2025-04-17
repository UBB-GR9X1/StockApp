namespace StockApp.Models
{
    public class Stock(string name, string symbol, string author_cnp, int price) : BaseStock(name, symbol, author_cnp)
    {
        public int Price { get; set; } = price;
    }
}
