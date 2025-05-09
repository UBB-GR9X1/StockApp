namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    public class LoanRequestService : ILoanRequestService
    {
        private readonly ILoanRequestRepository loanRequestRepository;
        private readonly IUserRepository userRepository;

        public LoanRequestService(ILoanRequestRepository loanRequestRepository, IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.loanRequestRepository = loanRequestRepository;
        }

        public async Task<string> GiveSuggestion(LoanRequest loanRequest)
        {

            User user = await this.userRepository.GetByCnpAsync(loanRequest.UserCnp) ?? throw new Exception("User not found");

            string givenSuggestion = string.Empty;

            if (loanRequest.Amount > user.Income * 10)
            {
                givenSuggestion = "Amount requested is too high for user income";
            }

            if (user.CreditScore < 300)
            {
                if (givenSuggestion.Length > 0)
                {
                    givenSuggestion += ", ";
                }

                givenSuggestion += "Credit score is too low";
            }

            if (user.RiskScore > 70)
            {
                if (givenSuggestion.Length > 0)
                {
                    givenSuggestion += ", ";
                }

                givenSuggestion += "User risk score is too high";
            }

            if (givenSuggestion.Length > 0)
            {
                givenSuggestion = "User does not qualify for loan: " + givenSuggestion;
            }

            return givenSuggestion;
        }

        public void SolveLoanRequest(LoanRequest loanRequest)
        {
            this.loanRequestRepository.SolveLoanRequest(loanRequest.Id);
        }

        public void DenyLoanRequest(LoanRequest loanRequest)
        {
            this.loanRequestRepository.DeleteLoanRequest(loanRequest.Id);
        }

        public bool PastUnpaidLoans(User user, LoanService loanService)
        {
            List<Loan> userLoanList;
            try
            {
                userLoanList = loanService.GetUserLoans(user.CNP);
            }
            catch (Exception)
            {
                userLoanList = new List<Loan>();
            }

            foreach (Loan loan in userLoanList)
            {
                if (loan.Status == "Active" && loan.RepaymentDate < DateTime.Today)
                {
                    return true;
                }
            }

            return false;
        }

        public float ComputeMonthlyDebtAmount(User user, LoanService loanServices)
        {
            List<Loan> loanList;
            try
            {
                loanList = loanServices.GetUserLoans(user.CNP);
            }
            catch (Exception)
            {
                loanList = new List<Loan>();
            }

            float monthlyDebtAmount = 0;

            foreach (Loan loan in loanList)
            {
                if (loan.Status == "Active")
                {
                    monthlyDebtAmount += loan.MonthlyPaymentAmount;
                }
            }

            return monthlyDebtAmount;
        }

        public List<LoanRequest> GetLoanRequests()
        {
            return this.loanRequestRepository.GetLoanRequests();
        }

        public List<LoanRequest> GetUnsolvedLoanRequests()
        {
            return this.loanRequestRepository.GetUnsolvedLoanRequests();
        }
    }
}
