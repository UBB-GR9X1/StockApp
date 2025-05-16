using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories
{
    public interface ITipsRepository
    {
        Task<List<Tip>> GetTipsForGivenUserAsync(string userCnp);
        Task<GivenTip> GiveHighBracketTipAsync(string userCnp);
        Task<GivenTip> GiveLowBracketTipAsync(string userCnp);
        Task<GivenTip> GiveMediumBracketTipAsync(string userCnp);
    }
}