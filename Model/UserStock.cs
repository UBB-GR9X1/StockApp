using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Model
{
    class UserStock: BaseStock
    {
        private int _quantity;
        public UserStock(string name, string symbol, string author_cnp, int quantity) : base(name, symbol, author_cnp)
        {
            _quantity = quantity;
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
    }
}
