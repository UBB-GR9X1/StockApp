namespace StockApp.Services
{
    using System.Threading.Tasks;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;

    public class StoreService : IStoreService
    {
        private readonly GemStoreRepository repository = new();

        /// <summary>
        /// Retrieves the CNP for the current user.
        /// </summary>
        /// <returns></returns>
        public string GetCnp()
        {
            return this.repository.GetCnp();
        }

        /// <summary>
        /// Checks if the user is a guest.
        /// </summary>
        /// <param name="cnp"></param>
        /// <returns></returns>
        public bool IsGuest(string cnp)
        {
            return this.repository.IsGuest(cnp);
        }

        /// <summary>
        /// Retrieves the current gem balance for the specified user.
        /// </summary>
        /// <param name="cnp"></param>
        /// <returns></returns>
        public int GetUserGemBalance(string cnp)
        {
            return this.repository.GetUserGemBalance(cnp);
        }

        /// <summary>
        /// Updates the gem balance for a given user.
        /// </summary>
        /// <param name="cnp"></param>
        /// <param name="newBalance"></param>
        public void UpdateUserGemBalance(string cnp, int newBalance)
        {
            this.repository.UpdateUserGemBalance(cnp, newBalance);
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
        public async Task<string> BuyGems(string userCNP, GemDeal deal, string selectedAccountId)
        {
            if (this.IsGuest(userCNP))
            {
                throw new GuestUserOperationException("Guests cannot buy gems.");
            }

            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, -deal.Price);
            if (!transactionSuccess)
            {
                throw new GemTransactionFailedException("Transaction failed. Please check your bank account balance.");
            }

            int currentBalance = this.GetUserGemBalance(userCNP);
            this.UpdateUserGemBalance(userCNP, currentBalance + deal.GemAmount);

            return $"Successfully purchased {deal.GemAmount} gems for {deal.Price}€";
        }

        /// <summary>
        /// Processes a bank transaction for selling gems.
        /// </summary>
        /// <param name="cnp"></param>
        /// <param name="gemAmount"></param>
        /// <param name="selectedAccountId"></param>
        /// <returns></returns>
        /// <exception cref="GuestUserOperationException"></exception>
        /// <exception cref="InsufficientGemsException"></exception>
        /// <exception cref="GemTransactionFailedException"></exception>
        public async Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId)
        {
            if (this.IsGuest(cnp))
            {
                throw new GuestUserOperationException("Guests cannot sell gems.");
            }

            int currentBalance = this.GetUserGemBalance(cnp);
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

            this.UpdateUserGemBalance(cnp, currentBalance - gemAmount);
            return $"Successfully sold {gemAmount} gems for {moneyEarned}€";
        }

        private static async Task<bool> ProcessBankTransaction(string accountId, double amount)
        {
            return true;
        }
    }
}
