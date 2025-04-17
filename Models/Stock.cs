namespace StockApp.Models
{
    public class Stock : BaseStock, IStock
    {
        public Stock(string name, string symbol, string authorCnp, int price)
            : base(name, symbol, authorCnp)
        {
            Price = price;
        }

        public int Price { get; set; }
    }
}
