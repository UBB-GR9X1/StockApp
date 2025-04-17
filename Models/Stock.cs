namespace StockApp.Models
{
    public class Stock(string name, string symbol, string authorCNP, int price, int quantity) : BaseStock(name, symbol, authorCNP)
    {
        public int Price { get; set; } = price;

        public int Quantity { get; set; } = quantity;

        public override string ToString()
        {
            return $"{this.Name} ({this.Symbol}) - x{this.Quantity} at {this.Price}";
        }
    }
}
