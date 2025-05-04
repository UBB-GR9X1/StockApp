namespace StockApp.Views.Pages
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;
    using StockApp.Services;
    using StockApp.Views.Components;

    public sealed partial class LoansView : Page
    {
        private readonly ILoanService service;
        private readonly ILoanCheckerService loanCheckerService;
        private readonly Func<LoanComponent> componentFactory;

        public LoansView(ILoanService loanService, ILoanCheckerService loanCheckerService, Func<LoanComponent> componentFactory)
        {
            this.InitializeComponent();

            this.service = loanService;
            this.loanCheckerService = loanCheckerService;
            this.componentFactory = componentFactory;

            this.loanCheckerService = new LoanCheckerService(this.service);
            this.loanCheckerService.LoansUpdated += this.OnLoansUpdated;
            this.loanCheckerService.Start();

            this.LoadLoans();
        }

        private void OnLoansUpdated(object sender, EventArgs e)
        {
            this.LoadLoans();
        }

        private void LoadLoans()
        {
            this.LoansContainer.Items.Clear();

            try
            {
                List<Loan> loans = this.service.GetLoans();
                foreach (Loan loan in loans)
                {
                    LoanComponent loanComponent = this.componentFactory();
                    loanComponent.SetLoanData(loan.Id, loan.UserCnp, loan.LoanAmount, loan.ApplicationDate,
                                              loan.RepaymentDate, loan.InterestRate, loan.NumberOfMonths, loan.MonthlyPaymentAmount,
                                              loan.Status, loan.MonthlyPaymentsCompleted, loan.RepaidAmount, loan.Penalty);

                    loanComponent.LoanUpdated += this.OnLoansUpdated;

                    this.LoansContainer.Items.Add(loanComponent);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error - LoadLoans: {exception.Message}");
            }
        }
    }
}
