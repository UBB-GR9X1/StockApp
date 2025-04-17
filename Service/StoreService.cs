namespace StockApp.Service
{
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repository;

    public class StoreService
    {
        private readonly GemStoreRepository repository = new GemStoreRepository();

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
            if (this.IsGuest(userCNP))
            {
                return "Guests cannot buy gems.";
            }

            bool transactionSuccess = await this.ProcessBankTransaction(selectedAccountId, -deal.Price);
            if (!transactionSuccess)
            {
                return "Transaction failed. Please check your bank account balance.";
            }

            int currentBalance = this.GetUserGemBalance(userCNP);
            this.UpdateUserGemBalance(userCNP, currentBalance + deal.GemAmount);

            return $"Successfully purchased {deal.GemAmount} gems for {deal.Price}€";
        }

        public async Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId)
        {
            if (this.IsGuest(cnp))
            {
                return "Guests cannot sell gems.";
            }

            int currentBalance = this.GetUserGemBalance(cnp);
            if (gemAmount > currentBalance)
            {
                return "Not enough Gems.";
            }

            double moneyEarned = gemAmount / 100.0;
            bool transactionSuccess = await this.ProcessBankTransaction(selectedAccountId, moneyEarned);
            if (!transactionSuccess)
            {
                return "Transaction failed. Unable to deposit funds.";
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
