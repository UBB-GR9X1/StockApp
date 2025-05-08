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
    using StockApp.ViewModels;

    public sealed partial class InvestmentsView : Page
    {
        private readonly InvestmentsViewModel _viewModel;

        public InvestmentsView(InvestmentsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
        }

        private async void UpdateCreditScoreCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.CreditScoreUpdateInvestmentsBasedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating credit score: {ex.Message}");
            }
        }

        private async void CalculateROICommand(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.CalculateAndUpdateROIAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating ROI: {ex.Message}");
            }
        }

        private async void CalculateRiskScoreCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.CalculateAndUpdateRiskScoreAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating risk score: {ex.Message}");
            }
        }

        private async void LoadInvestmentPortofolio(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.LoadPortfolioSummaryAsync(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading investment portfolio: {ex.Message}");
            }
        }
    }
}
