using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Repositories
{
    public interface IGemStoreRepository
    {
        string GetCnp();

        int GetUserGemBalance(string cnp);

        void UpdateUserGemBalance(string cnp, int newBalance);

        bool IsGuest(string cnp);
    }
}
