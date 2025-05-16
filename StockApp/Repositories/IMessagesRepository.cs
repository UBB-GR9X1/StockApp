using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace StockApp.Repositories
{
    public interface IMessagesRepository
    {
        Task<List<Message>> GetMessagesForUserAsync(string userCnp);
        Task GiveUserRandomMessageAsync(string userCnp);
        Task GiveUserRandomRoastMessageAsync(string userCnp);
    }
}