namespace StockApp.Views.Components
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;
    using StockApp.Services;

    public sealed partial class LoanRequestComponent : Page
    {
        private readonly ILoanRequestService loanRequestService;
        private readonly ILoanService loanServices;

        public event EventHandler LoanRequestSolved;

        public int RequestID { get; set; }

        public string RequestingUserCNP { get; set; }

        public float RequestedAmount { get; set; }

        public DateTime ApplicationDate { get; set; }

        public DateTime RepaymentDate { get; set; }

        public string State { get; set; }

        public string Suggestion { get; set; }

        public LoanRequestComponent(ILoanRequestService loanRequestService, ILoanService loanService)
        {
            this.loanRequestService = loanRequestService;
            this.loanServices = loanService;
            this.InitializeComponent();
        }

        private async void OnDenyClick(object sender, RoutedEventArgs e)
        {
            LoanRequest loanRequest = new LoanRequest(this.RequestID, this.RequestingUserCNP, this.RequestedAmount, this.ApplicationDate, this.RepaymentDate, this.State);
            this.loanRequestService.DeleteLoanRequest(loanRequest);
            this.LoanRequestSolved?.Invoke(this, EventArgs.Empty);
        }

        private async void OnApproveClick(object sender, RoutedEventArgs e)
        {
            LoanRequest loanRequest = new LoanRequest(this.RequestID, this.RequestingUserCNP, this.RequestedAmount, this.ApplicationDate, this.RepaymentDate, this.State);
            this.loanServices.AddLoan(loanRequest);
            this.loanRequestService.SolveLoanRequest(loanRequest);
            this.LoanRequestSolved?.Invoke(this, EventArgs.Empty);
        }

        public void SetRequestData(int id, string requestingUserCnp, float requestedAmount, DateTime applicationDate, DateTime repaymentDate, string state, string suggestion)
        {
            this.RequestID = id;
            this.RequestingUserCNP = requestingUserCnp;
            this.RequestedAmount = requestedAmount;
            this.ApplicationDate = applicationDate;
            this.RepaymentDate = repaymentDate;
            this.State = state;
            this.Suggestion = suggestion;

            this.IdTextBlock.Text = $"ID: {id}";
            this.RequestingUserCNPTextBlock.Text = $"User CNP: {requestingUserCnp}";
            this.RequestedAmountTextBlock.Text = $"Amount: {requestedAmount}";
            this.ApplicationDateTextBlock.Text = $"Application Date: {applicationDate:yyyy-MM-dd}";
            this.RepaymentDateTextBlock.Text = $"Repayment Date: {repaymentDate:yyyy-MM-dd}";
            this.SuggestionTextBlock.Text = $"{suggestion}";
        }
    }
}
