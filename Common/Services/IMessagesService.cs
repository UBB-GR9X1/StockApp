namespace Common.Services
{
    using Common.Models; // Added for Message model
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMessagesService
    {
        Task GiveMessageToUserAsync(string userCNP);
        Task<List<Message>> GetMessagesForUserAsync(string userCnp); // Added missing method
    }
}
