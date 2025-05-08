namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGemStoreRepository
    {
        Task<string> GetCnpAsync();

        Task<int> GetUserGemBalanceAsync(string cnp);

        Task UpdateUserGemBalanceAsync(string cnp, int newBalance);

        Task<bool> IsGuestAsync(string cnp);
    }
}
