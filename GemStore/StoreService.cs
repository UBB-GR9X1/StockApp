using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemStore.Models;

namespace GemStore
{
    public class StoreService
    {
        public async Task<string> BuyGems(string userType, GemDeal deal, string selectedAccountId)
        {
            if (userType == "Guest")
                return "Guests cannot buy gems.";

            // Simulate bank account transaction
            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, -deal.Price);

            if (transactionSuccess)
            {
                return $"Successfully purchased {deal.GemAmount} gems for {deal.Price}€";
            }
            else
            {
                return "Transaction failed. Please check your bank account balance.";
            }
        }

        public async Task<string> SellGems(string userType, int gemAmount, string selectedAccountId)
        {
            if (userType == "Guest")
                return "Guests cannot sell gems.";

            double moneyEarned = gemAmount / 100.0; // 100 Gems = 1€

            // Simulate bank account transaction
            bool transactionSuccess = await ProcessBankTransaction(selectedAccountId, moneyEarned);

            if (transactionSuccess)
            {
                return $"Successfully sold {gemAmount} gems for {moneyEarned}€";
            }
            else
            {
                return "Transaction failed. Unable to deposit funds.";
            }
        }

        private async Task<bool> ProcessBankTransaction(string accountId, double amount)
        {
            // Simulating bank transaction delay
            await Task.Delay(1000);

            // TODO: Integrate with the actual banking system
            return true; // Simulating successful transaction for now
        }
    }
}
