namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class StockPageService : IStockPageService
    {
        private readonly IStockPageRepository stockRepo;
        private readonly IUserRepository userRepo;
        private readonly ITransactionRepository transactionRepo;
        private string selectedStockName;
        Random randomNumberGenerator = new();

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
            var stock = await this.stockRepo.GetStockAsync(this.selectedStockName);
            return stock.Name;
        }

        /// <inheritdoc/>
        public async Task<string> GetStockSymbolAsync()
        {
            var stock = await this.stockRepo.GetStockAsync(this.selectedStockName);
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
        public async Task<UserStock> GetUserStockAsync()
        {
            if (IUserRepository.IsGuest)
            {
                throw new Exception("Guest users cannot own stocks.");
            }
            if (string.IsNullOrEmpty(this.selectedStockName))
            {
                throw new Exception("No stock selected.");
            }
            var userStock = await this.stockRepo.GetUserStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName);
            return userStock ?? throw new Exception("User stock not found.");
        }

        /// <inheritdoc/>
        public async Task<bool> BuyStockAsync(int quantity)
        {
            var selectedStock = await this.stockRepo.GetStockAsync(this.selectedStockName);
            int stockPrice = selectedStock.Price;
            int totalPrice = stockPrice * quantity;

            var user = await this.userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
            if (user?.GemBalance >= totalPrice)
            {
                user.GemBalance -= totalPrice;
                await this.userRepo.UpdateAsync(user.Id, user);

                int newPrice = stockPrice + ((randomNumberGenerator.Next(0, 20) - 5) * quantity);
                newPrice = Math.Max(newPrice, 20);

                await this.stockRepo.AddStockValueAsync(this.selectedStockName, newPrice);
                await this.stockRepo.AddOrUpdateUserStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName, quantity);

                await this.transactionRepo.AddTransactionAsync(
                    new TransactionLogTransaction(
                        selectedStock.Symbol,
                        this.selectedStockName,
                        "BUY",
                        quantity,
                        stockPrice,
                        DateTime.UtcNow,
                        user));

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> SellStockAsync(int quantity)
        {
            var selectedStock = await this.stockRepo.GetStockAsync(this.selectedStockName);
            int totalPrice = selectedStock.Price * quantity;
            int ownedStockCount = await this.stockRepo.GetOwnedStocksAsync(IUserRepository.CurrentUserCNP, this.selectedStockName);
            if (ownedStockCount >= quantity)
            {
                int newPrice = selectedStock.Price + ((this.randomNumberGenerator.Next(0, 10) - 5) * quantity);
                newPrice = Math.Max(newPrice, 20);

                await this.stockRepo.AddStockValueAsync(this.selectedStockName, newPrice);
                await this.stockRepo.AddOrUpdateUserStockAsync(IUserRepository.CurrentUserCNP, this.selectedStockName, ownedStockCount - quantity);

                var user = await this.userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
                user.GemBalance += totalPrice;
                await this.userRepo.UpdateAsync(user.Id, user);

                await this.transactionRepo.AddTransactionAsync(
                    new TransactionLogTransaction(
                        (await this.stockRepo.GetStockAsync(this.selectedStockName)).Symbol,
                        this.selectedStockName,
                        "SELL",
                        quantity,
                        selectedStock.Price,
                        DateTime.UtcNow,
                        user));

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
            var stock = await this.stockRepo.GetStockAsync(this.selectedStockName);
            return await this.userRepo.GetByCnpAsync(stock.AuthorCNP) ?? throw new Exception("User not found.");
        }
    }
}
