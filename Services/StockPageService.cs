namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    class StockPageService
    {
        StockPageRepository _repo;
        StockPageStock _stock;

        public StockPageService(string stockName)
        {
            _repo = new StockPageRepository();
            _stock = _repo.GetStock(stockName);
        }

        public bool IsGuest()
        {
            return _repo.IsGuest;
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
            return _repo.User.GemBalance;
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

            if (_repo.User.GemBalance >= totalPrice)
            {
                _repo.UpdateUserGems(_repo.User.GemBalance - totalPrice);
                Random r = new Random();
                int new_price = stockPrice + (r.Next(0, 20) - 5) * quantity;
                if (new_price < 20) new_price = 20;
                _repo.AddStockValue(_stock.Name, new_price);
                _repo.AddOrUpdateUserStock(_stock.Name, quantity);

                TransactionRepository repository = new TransactionRepository();
                repository.AddTransaction(new TransactionLogTransaction(_stock.Symbol, _stock.Name, "BUY", quantity, stockPrice, DateTime.UtcNow, _repo.User.CNP));

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
                _repo.AddStockValue(_stock.Name, new_price);
                _repo.AddOrUpdateUserStock(_stock.Name, -quantity);
                _repo.UpdateUserGems(_repo.User.GemBalance + totalPrice);

                TransactionRepository repository = new TransactionRepository();
                repository.AddTransaction(new TransactionLogTransaction(_stock.Symbol, _stock.Name, "SELL", quantity, stockPrice, DateTime.UtcNow, _repo.User.CNP));

                return true;
            }
            return false;
        }

        public bool GetFavorite()
        {
            return _repo.GetFavorite(_stock.Name);
        }

        public void ToggleFavorite(bool state)
        {
            _repo.ToggleFavorite(_stock.Name, state);
        }

        public string GetStockAuthor()
        {
            return _stock.AuthorCNP;
        }
    }
}
