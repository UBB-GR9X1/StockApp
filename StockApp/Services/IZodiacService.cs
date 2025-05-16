namespace StockApp.Services
{
    using System.Threading.Tasks;

    public interface IZodiacService
    {
        Task CreditScoreModificationBasedOnJokeAndCoinFlipAsync();

        void CreditScoreModificationBasedOnAttributeAndGravity();
    }
}
