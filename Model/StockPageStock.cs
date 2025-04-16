namespace StockApp.Model
{
    public class StockPageStock
    {
        private string _name;
        private string _symbol;
        private string _authroCnp;

        public StockPageStock(string name, string symbol, string authorCnp)
        {
            _name = name;
            _symbol = symbol;
            _authroCnp = authorCnp;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public string AuthorCnp
        {
            get { return _authroCnp; }
            set { _authroCnp = value; }
        }
    }
}
