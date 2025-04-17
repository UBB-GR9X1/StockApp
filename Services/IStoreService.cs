using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Service
{
    public interface IStoreService
    {
        string GetCnp();

        bool IsGuest(string cnp);

        int GetUserGemBalance(string cnp);

        void UpdateUserGemBalance(string cnp, int newBalance);

        Task<string> BuyGems(string cnp, IGemDeal deal, string selectedAccountId);

        Task<string> SellGems(string cnp, int gemAmount, string selectedAccountId);
    }
}
