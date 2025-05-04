namespace StockApp.Views.Components
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Services;

    public sealed partial class LoanComponent : Page
    {
        private readonly ILoanService loanServices;
        public event EventHandler LoanUpdated;

        private int loanID;
        private string userCNP;
        private float loanAmount;
        private DateTime applicationDate;
        private DateTime repaymentDate;
        private float interestRate;
        private int numberOfMonths;
        private float monthlyPaymentAmount;
        private string status;
        private int monthlyPaymentsCompleted;
        private float repaidAmount;
        private float penalty;

        public LoanComponent(ILoanService loanServices)
        {
            this.loanServices = loanServices;
            this.InitializeComponent();
        }

        public void SetLoanData(int loanID, string userCNP, float loanAmount, DateTime applicationDate,
                                DateTime repaymentDate, float interestRate, int noMonths, float monthlyPaymentAmount,
                                string state, int monthlyPaymentsCompleted, float repaidAmount, float penalty)
        {
            this.loanID = loanID;
            this.userCNP = userCNP;
            this.loanAmount = loanAmount;
            this.applicationDate = applicationDate;
            this.repaymentDate = repaymentDate;
            this.interestRate = interestRate;
            this.numberOfMonths = noMonths;
            this.monthlyPaymentAmount = monthlyPaymentAmount;
            this.status = state;
            this.monthlyPaymentsCompleted = monthlyPaymentsCompleted;
            this.repaidAmount = repaidAmount;
            this.penalty = penalty;

            this.LoanIDTextBlock.Text = $"Loan ID: {loanID}";
            this.UserCNPTextBlock.Text = $"User CNP: {userCNP}";
            this.LoanAmountTextBlock.Text = $"Amount: {loanAmount}";
            this.ApplicationDateTextBlock.Text = $"Applied: {applicationDate:yyyy-MM-dd}";
            this.RepaymentDateTextBlock.Text = $"Repay By: {repaymentDate:yyyy-MM-dd}";
            this.InterestRateTextBlock.Text = $"Interest: {interestRate}%";
            this.NoMonthsTextBlock.Text = $"Duration: {noMonths} months";
            this.MonthlyPaymentAmountTextBlock.Text = $"Monthly Payment: {monthlyPaymentAmount}";
            this.StateTextBlock.Text = $"State: {state}";
            this.MonthlyPaymentsCompletedTextBlock.Text = $"Payments Done: {monthlyPaymentsCompleted}";
            this.RepaidAmountTextBlock.Text = $"Repaid: {repaidAmount}";
            this.PenaltyTextBlock.Text = $"Penalty: {penalty}";
        }

        private async void OnSolveClick(object sender, RoutedEventArgs e)
        {
            this.loanServices.IncrementMonthlyPaymentsCompleted(this.loanID, this.penalty);
            this.LoanUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
