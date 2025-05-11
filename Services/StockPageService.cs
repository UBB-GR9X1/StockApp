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
        private readonly IStockPageRepository stockRepo;
        private readonly IUserRepository userRepo;
        private readonly ITransactionRepository transactionRepo;
        private string selectedStockName;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockPageService"/> class.
        /// </summary>
        public StockPageService(IStockPageRepository stockRepo, IUserRepository userRepo, ITransactionRepository transactionRepo)
        {
            this.stockRepo = stockRepo ?? throw new ArgumentNullException(nameof(stockRepo));
            this.userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            this.transactionRepo = transactionRepo ?? throw new ArgumentNullException(nameof(transactionRepo));
        }

        /// <inheritdoc/>
        public void SelectStock(Stock stock)
        {
            if (stock == null)
            {
                throw new Exception("No stock selected.");
            }

            this.selectedStockName = stock.Name;
        }

        /// <inheritdoc/>
        public async Task<string> GetStockNameAsync()
        {
            var stock = await this.stockRepo.GetStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName);
            return stock.Name;
        }

        /// <inheritdoc/>
        public async Task<string> GetStockSymbolAsync()
        {
            var stock = await this.stockRepo.GetStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName);
            return stock.Symbol;
        }

        /// <inheritdoc/>
        public async Task<int> GetUserBalanceAsync()
        {
            var user = await this.userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
            return user?.GemBalance ?? throw new Exception("User not found.");
        }

        /// <inheritdoc/>
        public async Task<List<int>> GetStockHistoryAsync()
        {
            return await this.stockRepo.GetStockHistoryAsync(this.selectedStockName);
        }

        /// <inheritdoc/>
        public async Task<int> GetOwnedStocksAsync()
        {
            return await this.stockRepo.GetOwnedStocksAsync(IUserRepository.CurrentUserCNP, this.selectedStockName);
        }

        /// <inheritdoc/>
        public async Task<bool> BuyStockAsync(int quantity)
        {
            var stockHistory = await this.GetStockHistoryAsync();
            int stockPrice = stockHistory.Last();
            int totalPrice = stockPrice * quantity;

            var user = await this.userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
            if (user?.GemBalance >= totalPrice)
            {
                await this.stockRepo.UpdateUserGemsAsync(IUserRepository.CurrentUserCNP, user.GemBalance - totalPrice);

                Random r = new Random();
                int newPrice = stockPrice + ((r.Next(0, 20) - 5) * quantity);
                newPrice = Math.Max(newPrice, 20);

                await this.stockRepo.AddStockValueAsync(this.selectedStockName, newPrice);
                await this.stockRepo.AddOrUpdateUserStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName, quantity);

                await transactionRepo.AddTransactionAsync(
                    new TransactionLogTransaction(
                        (await this.stockRepo.GetStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName)).Symbol,
                        this.selectedStockName,
                        "BUY",
                        quantity,
                        stockPrice,
                        DateTime.UtcNow,
                        IUserRepository.CurrentUserCNP));

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> SellStockAsync(int quantity)
        {
            var stockHistory = await this.GetStockHistoryAsync();
            int stockPrice = stockHistory.Last();
            int totalPrice = stockPrice * quantity;

            if (await this.stockRepo.GetOwnedStocksAsync(IUserRepository.CurrentUserCNP, this.selectedStockName) >= quantity)
            {
                Random r = new Random();
                int newPrice = stockPrice + ((r.Next(0, 10) - 5) * quantity);
                newPrice = Math.Max(newPrice, 20);

                await this.stockRepo.AddStockValueAsync(this.selectedStockName, newPrice);
                await this.stockRepo.AddOrUpdateUserStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName, -quantity);

                var user = await this.userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
                await this.stockRepo.UpdateUserGemsAsync(IUserRepository.CurrentUserCNP, user?.GemBalance + totalPrice ?? throw new Exception("User not found."));

                await this.transactionRepo.AddTransactionAsync(
                    new TransactionLogTransaction(
                        (await this.stockRepo.GetStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName)).Symbol,
                        this.selectedStockName,
                        "SELL",
                        quantity,
                        stockPrice,
                        DateTime.UtcNow,
                        IUserRepository.CurrentUserCNP));

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> GetFavoriteAsync()
        {
            return await this.stockRepo.GetFavoriteAsync(IUserRepository.CurrentUserCNP, this.selectedStockName);
        }

        /// <inheritdoc/>
        public async Task ToggleFavoriteAsync(bool state)
        {
            await this.stockRepo.ToggleFavoriteAsync(IUserRepository.CurrentUserCNP, this.selectedStockName, state);
        }

        /// <inheritdoc/>
        public async Task<User> GetStockAuthorAsync()
        {
            var stock = await this.stockRepo.GetStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName);
            return await this.userRepo.GetByCnpAsync(stock.AuthorCNP) ?? throw new Exception("User not found.");
        }
    }
}
