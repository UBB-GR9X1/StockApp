namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class StockPageService : IStockPageService
    {
        private readonly StockPageRepository stockRepo;
        private string selectedStockName;
        private readonly UserRepository userRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockPageService"/> class.
        /// </summary>
        public StockPageService()
        {
            this.stockRepo = new();
            this.userRepo = new();
        }

        /// <summary>
        /// Selects a stock for the user to view or interact with.
        /// </summary>
        /// <param name="stock"></param>
        /// <exception cref="Exception"></exception>
        public void SelectStock(Stock stock)
        {
            if (stock == null)
            {
                throw new Exception("No stock selected.");
            }

            this.selectedStockName = stock.Name;
        }

        /// <summary>
        /// Checks if the user is a guest.
        /// </summary>
        /// <returns></returns>
        public bool IsGuest()
        {
            return this.stockRepo.IsGuest;
        }

        /// <summary>
        /// Gets the name of the selected stock.
        /// </summary>
        /// <returns></returns>
        public string GetStockName()
        {
            return this.stockRepo.GetStock(this.selectedStockName).Name;
        }

        /// <summary>
        /// Gets the symbol of the selected stock.
        /// </summary>
        /// <returns></returns>
        public string GetStockSymbol()
        {
            return this.stockRepo.GetStock(this.selectedStockName).Symbol;
        }

        /// <summary>
        /// Gets the price of the selected stock.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetUserBalance()
        {
            return this.stockRepo.User?.GemBalance ?? throw new Exception("User not found.");
        }

        /// <summary>
        /// Gets the history of stock prices for the selected stock.
        /// </summary>
        /// <returns></returns>
        public List<int> GetStockHistory()
        {
            List<int> res = this.stockRepo.GetStockHistory(this.selectedStockName);

            // res.Reverse();
            return res;
        }

        /// <summary>
        /// Gets the number of stocks owned by the user.
        /// </summary>
        /// <returns></returns>
        public int GetOwnedStocks()
        {
            return this.stockRepo.GetOwnedStocks(this.selectedStockName);
        }

        /// <summary>
        /// Buys a specified quantity of the selected stock.
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool BuyStock(int quantity)
        {
            List<int> stockHistory = this.GetStockHistory();
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

        /// <summary>
        /// Sells a specified quantity of the selected stock.
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool SellStock(int quantity)
        {
            List<int> stockHistory = this.GetStockHistory();
            int stockPrice = stockHistory.Last();
            int totalPrice = stockPrice * quantity;
            if (this.stockRepo.GetOwnedStocks(this.selectedStockName) >= quantity)
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

        /// <summary>
        /// Gets the favorite status of the selected stock.
        /// </summary>
        /// <returns></returns>
        public bool GetFavorite()
        {
            return this.stockRepo.GetFavorite(this.selectedStockName);
        }

        /// <summary>
        /// Toggles the favorite status of the selected stock.
        /// </summary>
        /// <param name="state"></param>
        public void ToggleFavorite(bool state)
        {
            this.stockRepo.ToggleFavorite(this.selectedStockName, state);
        }

        /// <summary>
        /// Gets the author of the selected stock.
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetStockAuthor()
        {
            string authorCNP = this.stockRepo.GetStock(this.selectedStockName).AuthorCNP;
            return await this.userRepo.GetUserByCnpAsync(authorCNP);
        }
    }
}
