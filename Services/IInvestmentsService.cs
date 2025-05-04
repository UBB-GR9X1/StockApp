namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IInvestmentsService
    {
        public void CalculateAndUpdateRiskScore();
        public void CalculateAndUpdateROI();
        public void CreditScoreUpdateInvestmentsBased();
        public List<InvestmentPortfolio> GetPortfolioSummary();
    }
}
