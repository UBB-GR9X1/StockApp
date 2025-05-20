using Common.Models;

namespace BankApi.Repositories
{
    public interface IMessagesRepository
    {
        Task<List<Message>> GetMessagesForUserAsync(string userCnp);
        Task GiveUserRandomMessageAsync(string userCnp);
        Task GiveUserRandomRoastMessageAsync(string userCnp);
    }
}