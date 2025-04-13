using Catel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockNewsPage.Services;
using StockApp.Model;

namespace StockApp.Service
{
    class StockPageService
    {
        StockPageRepository _repo;
        StockPageStock _stock;

        public StockPageService(string stock_name)
        {
            _repo = new StockPageRepository();
            _stock = _repo.GetStock(stock_name);
        }

        public bool IsGuest()
        {
            return _repo.IsGuest();
        }

        public string GetStockName()
        {
            return _stock.Name;
        }
        public string GetStockSymbol()
        {
            return _stock.Symbol;
        }
        public int GetUserBalance()
        {
            return _repo.GetUser().GemBalance;
        }

        public List<int> GetStockHistory()
        {
            List<int> res = _repo.GetStockHistory(_stock.Name);
            // res.Reverse();
            return res;
        }

        public int GetOwnedStocks()
        {
            return _repo.GetOwnedStocks(_stock.Name);
        }

        public bool BuyStock(int quantity)
        {
            List<int> stockHistory = GetStockHistory();
            int stockPrice = stockHistory.Last();

            int totalPrice = stockPrice * quantity;

            if (_repo.GetUser().GemBalance >= totalPrice)
            {
                _repo.updateUserGems(_repo.GetUser().GemBalance - totalPrice);
                Random r = new Random();
                int new_price = stockPrice + (r.Next(0, 20) - 5) * quantity;
                if (new_price < 20) new_price = 20;
                _repo.addStockValue(_stock.Name, new_price);
                _repo.addOrUpdateUserStock(_stock.Name, quantity);

                TransactionRepository repository = new TransactionRepository();
                repository.AddTransaction(new Transaction(_stock.Symbol, _stock.Name, "BUY", quantity, stockPrice, DateTime.UtcNow, _repo.GetUser().CNP));

                return true;
            }
            return false;
        }

        public bool SellStock(int quantity)
        {
            List<int> stockHistory = GetStockHistory();
            int stockPrice = stockHistory.Last();
            int totalPrice = stockPrice * quantity;
            if (_repo.GetOwnedStocks(_stock.Name) >= quantity)
            {
                Random r = new Random();
                int new_price = stockPrice + (r.Next(0, 10) - 5) * quantity;
                if (new_price < 20) new_price = 20;
                _repo.addStockValue(_stock.Name, new_price);
                _repo.addOrUpdateUserStock(_stock.Name, -quantity);
                _repo.updateUserGems(_repo.GetUser().GemBalance + totalPrice);

                TransactionRepository repository = new TransactionRepository();
                repository.AddTransaction(new Transaction(_stock.Symbol, _stock.Name, "SELL", quantity, stockPrice, DateTime.UtcNow, _repo.GetUser().CNP));

                return true;
            }
            return false;
        }

        public bool getFavorite()
        {
            return _repo.GetFavorite(_stock.Name);
        }
        
        public void toggleFavorite(bool state)
        {
            _repo.ToggleFavorite(_stock.Name, state);
        }

        public string getStockAuthor()
        {
            return _stock.AuthorCNP;
        }
    }
}
