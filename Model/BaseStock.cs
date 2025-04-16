namespace StockApp.Model
{
    public class BaseStock
    {
        private string _name;
        private string _symbol;
        private string _author_cnp;

        public BaseStock(string name, string symbol, string authorCnp)
        {
            _name = name;
            _symbol = symbol;
            _author_cnp = authorCnp;
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

        public string AuthorCNP
        {
            get { return _author_cnp; }
            set { _author_cnp = value; }
        }
    }
}
