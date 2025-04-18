namespace StockApp.Models
{
    public class Stock(string name, string symbol, string authorCnp, int price) : BaseStock(name, symbol, authorCnp)
    {
        public int Price { get; set; } = price;
    }
}
