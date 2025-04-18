namespace StockApp.Services
{
    using System.Threading.Tasks;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;

    public class StoreService : IStoreService
    {
        private readonly GemStoreRepository repository = new();

        public string GetCnp()
        {
            return this.repository.GetCnp();
        }

        public bool IsGuest(string cnp)
        {
            return this.repository.IsGuest(cnp);
        }

        public int GetUserGemBalance(string cnp)
        {
            return this.repository.GetUserGemBalance(cnp);
        }

        public void UpdateUserGemBalance(string cnp, int newBalance)
        {
            this.repository.UpdateUserGemBalance(cnp, newBalance);
        }

        public async Task<string> BuyGems(string userCNP, GemDeal deal, string selectedAccountId)
        {
            if (IsGuest(userCNP))
            {
                throw new GuestUserOperationException("Guests cannot buy gems.");
            }

            bool transactionSuccess = await this.ProcessBankTransaction(selectedAccountId, -deal.Price);
            if (!transactionSuccess)
            {
                throw new GemTransactionFailedException("Transaction failed. Please check your bank account balance.");
            }

            int currentBalance = this.GetUserGemBalance(userCNP);
            this.UpdateUserGemBalance(userCNP, currentBalance + deal.GemAmount);

            return $"Successfully purchased {deal.GemAmount} gems for {deal.Price}€";
        }

        public async Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId)
        {
            if (IsGuest(cnp))
            {
                throw new GuestUserOperationException("Guests cannot sell gems.");
            }

            int currentBalance = this.GetUserGemBalance(cnp);
            if (gemAmount > currentBalance)
            {
                throw new InsufficientGemsException($"Not enough gems to sell. You have {currentBalance}, attempted to sell {gemAmount}.");
            }

            double moneyEarned = gemAmount / 100.0;
            bool transactionSuccess = await this.ProcessBankTransaction(selectedAccountId, moneyEarned);
            if (!transactionSuccess)
            {
                throw new GemTransactionFailedException("Transaction failed. Unable to deposit funds.");
            }

            this.UpdateUserGemBalance(cnp, currentBalance - gemAmount);
            return $"Successfully sold {gemAmount} gems for {moneyEarned}€";
        }

        private async Task<bool> ProcessBankTransaction(string accountId, double amount)
        {
            return true;
        }
    }
}
