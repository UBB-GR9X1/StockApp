namespace StockApp.Models
{
    public class UserStock(string name, string symbol, string author_cnp, int quantity) : BaseStock(name, symbol, author_cnp)
    {
        public int Quantity { get; set; } = quantity;
    }
}
