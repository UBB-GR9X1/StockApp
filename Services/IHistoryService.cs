namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IHistoryService
    {
        List<CreditScoreHistory> GetHistoryByUserCNP(string userCNP);

        List<CreditScoreHistory> GetHistoryWeekly(string userCNP);

        List<CreditScoreHistory> GetHistoryMonthly(string userCNP);

        List<CreditScoreHistory> GetHistoryYearly(string userCNP);
    }
}
