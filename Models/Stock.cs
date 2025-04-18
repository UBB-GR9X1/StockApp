namespace StockApp.Models
{
    public class Stock : BaseStock
    {
        public Stock(string name, string symbol, string authorCNP, int price, int quantity)
            : base(name, symbol, authorCNP)
        {
            Price = price;
            Quantity = quantity;
        }

        public int Price { get; set; }

        public int Quantity { get; set; }
    }
}
