namespace StockApp.Services.Api
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
            User user = await userRepository.GetByCnpAsync(userCNP) ?? throw new Exception("User not found");
            try
            {
                if (user.CreditScore >= 550)
                {
                    await messagesRepository.GiveUserRandomMessageAsync(userCNP);
                }
                else
                {
                    await messagesRepository.GiveUserRandomMessageAsync(userCNP);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message},User is not found");
            }
        }

        public async Task<List<Message>> GetMessagesForGivenUser(string userCnp)
        {
            return await messagesRepository.GetMessagesForUserAsync(userCnp);
        }
    }
}
