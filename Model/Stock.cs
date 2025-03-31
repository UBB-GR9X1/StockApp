using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Model
{
    class Stock : BaseStock
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
