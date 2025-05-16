namespace StockApp.Services.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;
    using StockApp.Repositories;
    using Common.Services;

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

            selectedStockName = stock.Name;
        }

        /// <inheritdoc/>
        public async Task<string> GetStockNameAsync()
        {
            var stock = await stockRepo.GetStockAsync(selectedStockName);
            return stock.Name;
        }

        /// <inheritdoc/>
        public async Task<string> GetStockSymbolAsync()
        {
            var stock = await stockRepo.GetStockAsync(selectedStockName);
            return stock.Symbol;
        }

        /// <inheritdoc/>
        public async Task<int> GetUserBalanceAsync()
        {
            var user = await userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
            return user?.GemBalance ?? throw new Exception("User not found.");
        }

        /// <inheritdoc/>
        public async Task<List<int>> GetStockHistoryAsync()
        {
            return await stockRepo.GetStockHistoryAsync(selectedStockName);
        }

        /// <inheritdoc/>
        public async Task<int> GetOwnedStocksAsync()
        {
            return await stockRepo.GetOwnedStocksAsync(IUserRepository.CurrentUserCNP, selectedStockName);
        }

        /// <inheritdoc/>
        public async Task<UserStock> GetUserStockAsync()
        {
            if (IUserRepository.IsGuest)
            {
                throw new Exception("Guest users cannot own stocks.");
            }
            if (string.IsNullOrEmpty(selectedStockName))
            {
                throw new Exception("No stock selected.");
            }
            var userStock = await stockRepo.GetUserStockAsync(IUserRepository.CurrentUserCNP, selectedStockName);
            return userStock ?? throw new Exception("User stock not found.");
        }

        /// <inheritdoc/>
        public async Task<bool> BuyStockAsync(int quantity)
        {
            var selectedStock = await stockRepo.GetStockAsync(selectedStockName);
            int stockPrice = selectedStock.Price;
            int totalPrice = stockPrice * quantity;
            int ownedStockCount = await stockRepo.GetOwnedStocksAsync(IUserRepository.CurrentUserCNP, selectedStockName);
            var user = await userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
            if (user?.GemBalance >= totalPrice)
            {
                user.GemBalance -= totalPrice;
                await userRepo.UpdateAsync(user.Id, user);

                int newPrice = stockPrice + (randomNumberGenerator.Next(0, 20) - 5) * quantity;
                newPrice = Math.Max(newPrice, 20);

                await stockRepo.AddStockValueAsync(selectedStockName, newPrice);
                await stockRepo.AddOrUpdateUserStockAsync(IUserRepository.CurrentUserCNP, selectedStockName, ownedStockCount + quantity);

                await transactionRepo.AddTransactionAsync(
                    new TransactionLogTransaction(
                        selectedStock.Symbol,
                        selectedStockName,
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
            var selectedStock = await stockRepo.GetStockAsync(selectedStockName);
            int totalPrice = selectedStock.Price * quantity;
            int ownedStockCount = await stockRepo.GetOwnedStocksAsync(IUserRepository.CurrentUserCNP, selectedStockName);
            if (ownedStockCount >= quantity)
            {
                int newPrice = selectedStock.Price + (randomNumberGenerator.Next(0, 10) - 5) * quantity;
                newPrice = Math.Max(newPrice, 20);

                await stockRepo.AddStockValueAsync(selectedStockName, newPrice);
                await stockRepo.AddOrUpdateUserStockAsync(IUserRepository.CurrentUserCNP, selectedStockName, ownedStockCount - quantity);

                var user = await userRepo.GetByCnpAsync(IUserRepository.CurrentUserCNP);
                user.GemBalance += totalPrice;
                await userRepo.UpdateAsync(user.Id, user);

                await transactionRepo.AddTransactionAsync(
                    new TransactionLogTransaction(
                        (await stockRepo.GetStockAsync(selectedStockName)).Symbol,
                        selectedStockName,
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
            return await stockRepo.GetFavoriteAsync(IUserRepository.CurrentUserCNP, selectedStockName);
        }

        /// <inheritdoc/>
        public async Task ToggleFavoriteAsync(bool state)
        {
            await stockRepo.ToggleFavoriteAsync(IUserRepository.CurrentUserCNP, selectedStockName, state);
        }

        /// <inheritdoc/>
        public async Task<User> GetStockAuthorAsync()
        {
            var stock = await stockRepo.GetStockAsync(selectedStockName);
            return await userRepo.GetByCnpAsync(stock.AuthorCNP) ?? throw new Exception("User not found.");
        }
    }
}
