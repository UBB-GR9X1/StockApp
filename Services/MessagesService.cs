namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using Src.Data;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    public class MessagesService : IMessagesService
    {
        private readonly MessagesRepository messagesRepository;
        private readonly IUserRepository userRepository;

        public MessagesService(MessagesRepository messagesRepository, IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.messagesRepository = messagesRepository;
        }

        public async void GiveMessageToUser(string userCNP)
        {
            DatabaseConnection dbConn = new DatabaseConnection();
            User user = await this.userRepository.GetByCnpAsync(userCNP) ?? throw new Exception("User not found");
            try
            {
                if (user.CreditScore >= 550)
                {
                    this.messagesRepository.GiveUserRandomMessage(userCNP);
                }
                else
                {
                    this.messagesRepository.GiveUserRandomRoastMessage(userCNP);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message},User is not found");
            }
        }

        public List<Message> GetMessagesForGivenUser(string userCnp)
        {
            return this.messagesRepository.GetMessagesForGivenUser(userCnp);
        }
    }
}
