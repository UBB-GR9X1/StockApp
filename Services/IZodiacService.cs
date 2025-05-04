namespace StockApp.Services
{
    using System.Threading.Tasks;

    public interface IZodiacService
    {
        public Task CreditScoreModificationBasedOnJokeAndCoinFlipAsync();
        public void CreditScoreModificationBasedOnAttributeAndGravity();
    }
}
