using StockApp.StockPage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.StockPage
{
    class StockPageService
    {
        StockPageRepository _repo;
        Stock _stock;

        public StockPageService(string stock_name)
        {
            _repo = new StockPageRepository();
            _stock = _repo.GetStock(stock_name);
        }

        public string GetStockName()
        {
            return _stock.Name;
        }
        public string GetStockSymbol()
        {
            return _stock.Symbol;
        }
    }
}
