namespace StockApp.Views.Pages
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Src.Data;
    using Src.Model;
    using StockApp.Repositories;
    using StockApp.Services;
    using StockApp.Views.Components;

    public sealed partial class InvestmentsView : Page
    {
        private DatabaseConnection dbConnection;
        private InvestmentsRepository investmentsRepository;
        private InvestmentsService investmentsService;
        private UserRepository userRepository;

        public InvestmentsView()
        {
            this.dbConnection = new DatabaseConnection();
            this.investmentsRepository = new InvestmentsRepository(this.dbConnection);
            this.userRepository = new UserRepository();
            this.investmentsService = new InvestmentsService(this.userRepository, this.investmentsRepository);

            this.InitializeComponent();
            this.LoadInvestmentPortofolio();
        }

        private async void UpdateCreditScoreCommand(object sender, RoutedEventArgs e)
        {
            this.investmentsService.CreditScoreUpdateInvestmentsBased();
        }

        private async void CalculateROICommand(object sender, RoutedEventArgs e)
        {
            this.investmentsService.CalculateAndUpdateROI();
        }

        private async void CalculateRiskScoreCommand(object sender, RoutedEventArgs e)
        {
            this.investmentsService.CalculateAndUpdateRiskScore();
            this.LoadInvestmentPortofolio();
        }

        private void LoadInvestmentPortofolio()
        {
            this.UsersPortofolioContainer.Items.Clear();
            try
            {
                List<InvestmentPortfolio> usersInvestmentPortofolioo = this.investmentsService.GetPortfolioSummary();

                foreach (var userPortofolio in usersInvestmentPortofolioo)
                {
                    InvestmentComponent investmentComponent = new InvestmentComponent();
                    investmentComponent.SetPortfolioSummary(userPortofolio);

                    this.UsersPortofolioContainer.Items.Add(investmentComponent);
                }
            }
            catch (Exception)
            {
                this.UsersPortofolioContainer.Items.Add("There are no user investments.");
            }
        }
    }
}
