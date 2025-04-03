using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.StockPage.Model
{
    class Stock
    {
        private string _name;
        private string _symbol;
        private string _authro_cnp;

        public Stock(string name, string symbol, string author_cnp)
        {
            this._name = name;
            this._symbol = symbol;
            this._authro_cnp = author_cnp;
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
