namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IInvestmentsService
    {
        void CalculateAndUpdateRiskScore();

        void CalculateAndUpdateROI();

        void CreditScoreUpdateInvestmentsBased();

        List<InvestmentPortfolio> GetPortfolioSummary();
    }
}
