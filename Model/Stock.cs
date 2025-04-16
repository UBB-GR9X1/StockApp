namespace StockApp.Model
{
    public class Stock : BaseStock
    {
        private int _price;

        public Stock(string name, string symbol, string authorCnp, int price) : base(name, symbol, authorCnp)
        {
            _price = price;
        }
        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }
    }
}
