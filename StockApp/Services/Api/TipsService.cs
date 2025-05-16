namespace StockApp.Services.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Services;
    using StockApp.Repositories;

    public class TipsService : ITipsService
    {
        private ITipsRepository tipsRepository;
        private IUserRepository userRepository;

        public TipsService(ITipsRepository tipsRepository, IUserRepository userRepository)
        {
            this.tipsRepository = tipsRepository;
            this.userRepository = userRepository;
        }

        public async Task GiveTipToUser(string userCNP)
        {

            try
            {
                User user = await userRepository.GetByCnpAsync(userCNP) ?? throw new Exception("User not found");
                if (user.CreditScore < 300)
                {
                    await tipsRepository.GiveLowBracketTipAsync(userCNP);
                }
                else if (user.CreditScore < 550)
                {
                    await tipsRepository.GiveMediumBracketTipAsync(userCNP);
                }
                else if (user.CreditScore > 549)
                {
                    await tipsRepository.GiveHighBracketTipAsync(userCNP);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message},User is not found");
            }
        }

        public async Task<List<Tip>> GetTipsForGivenUser(string userCnp)
        {
            return await tipsRepository.GetTipsForGivenUserAsync(userCnp);
        }
    }
}
