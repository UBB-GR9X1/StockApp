namespace StockApp.Models
{
    public class UserStock(string name, string symbol, string authorCnp, int quantity) : BaseStock(name, symbol, authorCnp)
    {
        public int Quantity { get; set; } = quantity;
    }
}
