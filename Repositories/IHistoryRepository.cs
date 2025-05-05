namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IHistoryRepository
    {
        List<CreditScoreHistory> GetHistoryForUser(string userCNP);
    }
}
