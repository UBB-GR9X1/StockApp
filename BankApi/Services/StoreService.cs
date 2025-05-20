namespace BankApi.Services
{
    using BankApi.Repositories;
    using Common.Exceptions;
    using Common.Models;
    using Common.Services;
    using System;
    using System.Threading.Tasks;

    public class StoreService(IGemStoreRepository repository, IUserRepository userRepository) : IStoreService
    {
        private readonly IGemStoreRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));


        /// <summary>
        /// Retrieves the current gem balance for the specified user.
        /// </summary>
        /// <param name="userCNP"></param>
        /// <returns></returns>
        public async Task<int> GetUserGemBalanceAsync(string userCNP)
        {
            return await _repository.GetUserGemBalanceAsync(userCNP);
        }

        /// <summary>
        /// Updates the gem balance for a given user.
        /// </summary>
        /// <param name="userCNP"></param>
        /// <param name="newBalance"></param>
        public async Task UpdateUserGemBalanceAsync(int newBalance, string userCNP)
        {
            await _repository.UpdateUserGemBalanceAsync(userCNP, newBalance);
        }

        /// <summary>
        /// Processes a bank transaction for buying or selling gems.
        /// </summary>
        /// <param name="userCNP"></param>
        /// <param name="deal"></param>
        /// <param name="selectedAccountId"></param>
        /// <returns></returns>
        /// <exception cref="GuestUserOperationException"></exception>
        /// <exception cref="GemTransactionFailedException"></exception>
        public async Task<string> BuyGems(GemDeal deal, string selectedAccountId, string userCNP)
        {
            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, -deal.Price);
            if (!transactionSuccess)
            {
                throw new GemTransactionFailedException("Transaction failed. Please check your bank account balance.");
            }

            int currentBalance = await GetUserGemBalanceAsync(userCNP);
            await UpdateUserGemBalanceAsync(currentBalance + deal.GemAmount, userCNP);

            return $"Successfully purchased {deal.GemAmount} gems for {deal.Price}€";
        }

        /// <summary>
        /// Processes a bank transaction for selling gems.
        /// </summary>
        /// <param name="userCNP"> The CNP of the user.</param>
        /// <param name="gemAmount"> The amount of gems to sell.</param>
        /// <param name="selectedAccountId"> The selected account ID for the transaction.</param>
        /// <returns></returns>
        /// <exception cref="GuestUserOperationException"></exception>
        /// <exception cref="InsufficientGemsException"></exception>
        /// <exception cref="GemTransactionFailedException"></exception>
        public async Task<string> SellGems(int gemAmount, string selectedAccountId, string userCNP)
        {
            int currentBalance = await GetUserGemBalanceAsync(userCNP);
            if (gemAmount > currentBalance)
            {
                throw new InsufficientGemsException($"Not enough gems to sell. You have {currentBalance}, attempted to sell {gemAmount}.");
            }

            double moneyEarned = gemAmount / 100.0;
            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, moneyEarned);
            if (!transactionSuccess)
            {
                throw new GemTransactionFailedException("Transaction failed. Unable to deposit funds.");
            }

            await UpdateUserGemBalanceAsync(currentBalance - gemAmount, userCNP);
            return $"Successfully sold {gemAmount} gems for {moneyEarned}€";
        }

        private static async Task<bool> ProcessBankTransaction(string accountId, double amount)
        {
            return true;
        }
    }
}
