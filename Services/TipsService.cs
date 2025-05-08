namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Repositories;
    using StockApp.Models;

    public class TipsService : ITipsService
    {
        private TipsRepository tipsRepository;

        public TipsService(TipsRepository tipsRepository)
        {
            this.tipsRepository = tipsRepository;
        }

        public void GiveTipToUser(string userCNP)
        {
            UserRepository userRepository = new UserRepository();

            try
            {
                int userCreditScore = userRepository.GetUserByCnpAsync(userCNP).Result.CreditScore;
                if (userCreditScore < 300)
                {
                    this.tipsRepository.GiveUserTipForLowBracket(userCNP);
                }
                else if (userCreditScore < 550)
                {
                    this.tipsRepository.GiveUserTipForMediumBracket(userCNP);
                }
                else if (userCreditScore > 549)
                {
                    this.tipsRepository.GiveUserTipForHighBracket(userCNP);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message},User is not found");
            }
        }

        public List<Tip> GetTipsForGivenUser(string userCnp)
        {
            return this.tipsRepository.GetTipsForGivenUser(userCnp);
        }
    }
}
