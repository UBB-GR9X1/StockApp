using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Model
{
    class BaseStock
    {
        private string _name;
        private string _symbol;
        private string _author_cnp;

        public BaseStock(string name, string symbol, string author_cnp)
        {
            _name = name;
            _symbol = symbol;
            _author_cnp = author_cnp;
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
