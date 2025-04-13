namespace StockApp.Model
{
    public class StockPageStock
    {
        private string _name;
        private string _symbol;
        private string _authro_cnp;

        public StockPageStock(string name, string symbol, string author_cnp)
        {
            _name = name;
            _symbol = symbol;
            _authro_cnp = author_cnp;
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
            get { return _authro_cnp; }
            set { _authro_cnp = value; }
        }
    }
}
