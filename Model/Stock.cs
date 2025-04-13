namespace StockApp.Model
{
    public class Stock : BaseStock
    {
        private int _price;

        public Stock(string name, string symbol, string author_cnp, int price) : base(name, symbol, author_cnp)
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
