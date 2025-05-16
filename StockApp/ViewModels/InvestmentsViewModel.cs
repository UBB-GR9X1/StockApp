namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Services;

    public class InvestmentsViewModel
    {
        private readonly IInvestmentsService _investmentsService;

        public ObservableCollection<InvestmentPortfolio> UsersPortofolio { get; set; }

        public InvestmentsViewModel(IInvestmentsService investmentsService)
        {
            _investmentsService = investmentsService ?? throw new ArgumentNullException(nameof(investmentsService));
            UsersPortofolio = new ObservableCollection<InvestmentPortfolio>();
        }

        public async Task CalculateAndUpdateRiskScoreAsync()
        {
            try
            {
                await _investmentsService.CalculateAndUpdateRiskScoreAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating risk score: {ex.Message}");
                throw;
            }
        }

        public async Task CalculateAndUpdateROIAsync()
        {
            try
            {
                await _investmentsService.CalculateAndUpdateROIAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating ROI: {ex.Message}");
                throw;
            }
        }

        public async Task CreditScoreUpdateInvestmentsBasedAsync()
        {
            try
            {
                await _investmentsService.CreditScoreUpdateInvestmentsBasedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating credit score: {ex.Message}");
                throw;
            }
        }

        public async Task LoadPortfolioSummaryAsync(string userCNP)
        {
            try
            {
                UsersPortofolio.Clear();
                var portfoliosSummary = await _investmentsService.GetPortfolioSummaryAsync();

                foreach (var userPortfolio in portfoliosSummary)
                {
                    UsersPortofolio.Add(userPortfolio);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading portfolio summary: {ex.Message}");
                throw;
            }
        }
    }
}
