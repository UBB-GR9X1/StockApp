namespace StockApp.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using StockApp.Commands;
    using StockApp.Services;

    public class ZodiacFeatureViewModel
    {
        private readonly IZodiacService zodiacService;

        private readonly ICommand runZodiacFeaturesCommand;

        public ICommand RunZodiacFeaturesCommand
        {
            get
            {
                return this.runZodiacFeaturesCommand;
            }
        }

        public ZodiacFeatureViewModel(IZodiacService zodiacService)
        {
            this.zodiacService = zodiacService ?? throw new ArgumentNullException(nameof(zodiacService));
            this.runZodiacFeaturesCommand = new RelayCommand(async (object sender) => await this.RunZodiacFeatures());
        }

        public async Task RunZodiacFeatures()
        {
            await this.zodiacService.CreditScoreModificationBasedOnJokeAndCoinFlipAsync();
            this.zodiacService.CreditScoreModificationBasedOnAttributeAndGravity();
        }

    }
}
