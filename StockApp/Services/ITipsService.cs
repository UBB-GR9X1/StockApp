namespace StockApp.Services
{
    using System.Threading.Tasks;

    public interface ITipsService
    {
        Task GiveTipToUser(string userCNP);
    }
}
