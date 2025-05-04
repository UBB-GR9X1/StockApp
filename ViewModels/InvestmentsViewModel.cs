using System;
using System.Collections.ObjectModel;
using Src.Model;
using StockApp.Services;

namespace StockApp.ViewModels
{
    public class InvestmentsViewModel
    {
        private readonly IInvestmentsService investmentsService;

        public ObservableCollection<InvestmentPortfolio> UsersPortofolio { get; set; }

        public InvestmentsViewModel(IInvestmentsService investmentsService)
        {
            this.investmentsService = investmentsService ?? throw new ArgumentNullException(nameof(investmentsService));
            this.UsersPortofolio = new ObservableCollection<InvestmentPortfolio>();
        }

        public void CalculateAndUpdateRiskScore()
        {
            this.investmentsService.CalculateAndUpdateRiskScore();
        }

        public void CalculateAndUpdateROI()
        {
            this.investmentsService.CalculateAndUpdateROI();
        }

        public void CreditScoreUpdateInvestmentsBased()
        {
            this.investmentsService.CreditScoreUpdateInvestmentsBased();
        }

        public void LoadPortfolioSummary(string userCNP)
        {
            try
            {
                var portfoliosSummary = this.investmentsService.GetPortfolioSummary();

                foreach (var userPortfolio in portfoliosSummary)
                {
                    this.UsersPortofolio.Add(userPortfolio);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
        }
    }
}
