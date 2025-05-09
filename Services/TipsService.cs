namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    public class TipsService : ITipsService
    {
        private TipsRepository tipsRepository;
        private IUserRepository userRepository;

        public TipsService(TipsRepository tipsRepository)
        {
            this.tipsRepository = tipsRepository;
        }

        public async Task GiveTipToUser(string userCNP)
        {

            try
            {
                User user = await this.userRepository.GetByCnpAsync(userCNP) ?? throw new Exception("User not found");
                if (user.CreditScore < 300)
                {
                    this.tipsRepository.GiveUserTipForLowBracket(userCNP);
                }
                else if (user.CreditScore < 550)
                {
                    this.tipsRepository.GiveUserTipForMediumBracket(userCNP);
                }
                else if (user.CreditScore > 549)
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
