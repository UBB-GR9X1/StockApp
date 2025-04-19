namespace StockApp.Services
{
    using System;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;
    using StockApp.Exceptions;

    public class StoreService : IStoreService
    {
        private readonly GemStoreRepository repository = new ();

        // public void PopulateHardcodedCnps() => repository.PopulateHardcodedCnps();

        // public void PopulateUserTable() => repository.PopulateUserTable();

        public string GetCnp()
        {
            return repository.GetCnp();
        }

        public bool IsGuest(string cnp)
        {
            return repository.IsGuest(cnp);
        }

        public int GetUserGemBalance(string cnp)
        {
            return repository.GetUserGemBalance(cnp);
        }

        public void UpdateUserGemBalance(string cnp, int newBalance)
        {
            repository.UpdateUserGemBalance(cnp, newBalance);
        }

        public async Task<string> BuyGems(string cnp, GemDeal deal, string selectedAccountId)
        {
            if (IsGuest(cnp))
            {
                throw new GuestUserOperationException("Guests cannot buy gems.");
            }

            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, -deal.Price);
            if (!transactionSuccess)
            {
                throw new GemTransactionFailedException("Transaction failed. Please check your bank account balance.");
            }

            int currentBalance = GetUserGemBalance(cnp);
            UpdateUserGemBalance(cnp, currentBalance + deal.GemAmount);

            return $"Successfully purchased {deal.GemAmount} gems for {deal.Price}€";
        }

        public async Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId)
        {
            if (IsGuest(cnp))
            {
                throw new GuestUserOperationException("Guests cannot sell gems.");
            }

            int currentBalance = GetUserGemBalance(cnp);
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

            UpdateUserGemBalance(cnp, currentBalance - gemAmount);
            return $"Successfully sold {gemAmount} gems for {moneyEarned}€";
        }

        private async Task<bool> ProcessBankTransaction(string accountId, double amount)
        {
            await Task.Delay(1000);
            return true;
        }
    }
}
