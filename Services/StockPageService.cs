namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using StockApp.Models;
    using StockApp.Repositories;

    class StockPageService
    {
        private StockPageRepository stockRepo;
        private string selectedStockName;
        private UserRepository userRepo;

        public StockPageService()
        {
            this.stockRepo = new();
            this.userRepo = new();
        }

        public void SelectStock(Stock stock)
        {
            if (stock == null)
            {
                throw new Exception("No stock selected.");
            }

            this.selectedStockName = stock.Name;
        }

        public bool IsGuest()
        {
            return stockRepo.IsGuest;
        }

        public string GetStockName()
        {
            return this.stockRepo.GetStock(this.selectedStockName).Name;
        }

        public string GetStockSymbol()
        {
            return this.stockRepo.GetStock(this.selectedStockName).Symbol;
        }

        public int GetUserBalance()
        {
            return this.stockRepo.User?.GemBalance ?? throw new Exception("User not found.");
        }

        public List<int> GetStockHistory()
        {
            List<int> res = this.stockRepo.GetStockHistory(this.selectedStockName);
            // res.Reverse();
            return res;
        }

        public int GetOwnedStocks()
        {
            return this.stockRepo.GetOwnedStocks(this.selectedStockName);
        }

        public bool BuyStock(int quantity)
        {
            List<int> stockHistory = GetStockHistory();
            int stockPrice = stockHistory.Last();

            int totalPrice = stockPrice * quantity;

            if (this.stockRepo.User?.GemBalance >= totalPrice)
            {
                this.stockRepo.UpdateUserGems(this.stockRepo.User.GemBalance - totalPrice);
                Random r = new Random();
                int new_price = stockPrice + ((r.Next(0, 20) - 5) * quantity);
                if (new_price < 20)
                {
                    new_price = 20;
                }

                this.stockRepo.AddStockValue(this.selectedStockName, new_price);
                this.stockRepo.AddOrUpdateUserStock(this.selectedStockName, quantity);

                TransactionRepository repository = new TransactionRepository();
                repository.AddTransaction(
                    new TransactionLogTransaction(
                        this.stockRepo.GetStock(this.selectedStockName).Symbol,
                        this.selectedStockName,
                        "BUY",
                        quantity,
                        stockPrice,
                        DateTime.UtcNow,
                        this.stockRepo.User.CNP));

                return true;
            }

            return false;
        }

        public bool SellStock(int quantity)
        {
            List<int> stockHistory = GetStockHistory();
            int stockPrice = stockHistory.Last();
            int totalPrice = stockPrice * quantity;
            if (stockRepo.GetOwnedStocks(selectedStockName) >= quantity)
            {
                Random r = new Random();
                int new_price = stockPrice + ((r.Next(0, 10) - 5) * quantity);
                if (new_price < 20)
                {
                    new_price = 20;
                }

                this.stockRepo.AddStockValue(this.selectedStockName, new_price);
                this.stockRepo.AddOrUpdateUserStock(this.selectedStockName, -quantity);
                this.stockRepo.UpdateUserGems(this.stockRepo.User?.GemBalance + totalPrice ?? throw new Exception("User not found."));

                TransactionRepository repository = new TransactionRepository();
                repository.AddTransaction(new TransactionLogTransaction(
                    this.stockRepo.GetStock(this.selectedStockName).Symbol,
                    this.selectedStockName,
                    "SELL",
                    quantity,
                    stockPrice,
                    DateTime.UtcNow,
                    this.stockRepo.User?.CNP ?? throw new Exception("User not found.")));

                return true;
            }

            return false;
        }

        public bool GetFavorite()
        {
            return this.stockRepo.GetFavorite(this.selectedStockName);
        }

        public void ToggleFavorite(bool state)
        {
            this.stockRepo.ToggleFavorite(this.selectedStockName, state);
        }

        public User GetStockAuthor()
        {
            string authorCNP = this.stockRepo.GetStock(this.selectedStockName).AuthorCNP;
            return this.userRepo.GetUserByCnpAsync(authorCNP).Result;
        }
    }
}
