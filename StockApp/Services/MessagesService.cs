namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class MessagesService : IMessagesService
    {
        private readonly IMessagesRepository messagesRepository;
        private readonly IUserRepository userRepository;

        public MessagesService(IMessagesRepository messagesRepository, IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.messagesRepository = messagesRepository;
        }

        public async void GiveMessageToUser(string userCNP)
        {
            User user = await this.userRepository.GetByCnpAsync(userCNP) ?? throw new Exception("User not found");
            try
            {
                if (user.CreditScore >= 550)
                {
                    await this.messagesRepository.GiveUserRandomMessageAsync(userCNP);
                }
                else
                {
                    await this.messagesRepository.GiveUserRandomMessageAsync(userCNP);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message},User is not found");
            }
        }

        public async Task<List<Message>> GetMessagesForGivenUser(string userCnp)
        {
            return await this.messagesRepository.GetMessagesForUserAsync(userCnp);
        }
    }
}
