namespace StockApp.Models
{
    public class UserStock : BaseStock, IUserStock
    {
        public int Quantity { get; set; }

        public UserStock(
            string name,
            string symbol,
            string authorCnp,
            int quantity)
            : base(name, symbol, authorCnp)
        {
            Quantity = quantity;
        }
    }
}
