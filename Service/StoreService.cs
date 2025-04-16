using System.Threading.Tasks;
using StockApp.Model;
using StockApp.Repository;

namespace StockApp.Service
{
    public class StoreService
    {
        private readonly GemStoreRepository repository = new GemStoreRepository();

        //public void PopulateHardcodedCnps() => repository.PopulateHardcodedCnps();

        //public void PopulateUserTable() => repository.PopulateUserTable();

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

        public async Task<string> BuyGems(string cnp, GemStoreGemDeal deal, string selectedAccountId)
        {
            if (IsGuest(cnp))
                return "Guests cannot buy gems.";

            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, -deal.Price);
            if (!transactionSuccess)
                return "Transaction failed. Please check your bank account balance.";

            int currentBalance = GetUserGemBalance(cnp);
            UpdateUserGemBalance(cnp, currentBalance + deal.GemAmount);

            return $"Successfully purchased {deal.GemAmount} gems for {deal.Price}€";
        }

        public async Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId)
        {
            if (IsGuest(cnp))
                return "Guests cannot sell gems.";

            int currentBalance = GetUserGemBalance(cnp);
            if (gemAmount > currentBalance)
                return "Not enough Gems.";

            double moneyEarned = gemAmount / 100.0;
            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, moneyEarned);
            if (!transactionSuccess)
                return "Transaction failed. Unable to deposit funds.";

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
