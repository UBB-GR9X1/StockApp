namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Data;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    public class LoanRequestService : ILoanRequestService
    {
        private readonly ILoanRequestRepository loanRequestRepository;

        public LoanRequestService(ILoanRequestRepository loanRequestRepository)
        {
            this.loanRequestRepository = loanRequestRepository;
        }

        public async Task<string> GiveSuggestion(LoanRequest loanRequest)
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            UserRepository userRepository = new UserRepository();
            LoanService loanService = new LoanService(new LoanRepository(dbConnection));

            User user = userRepository.GetUserByCnpAsync(loanRequest.UserCnp).Result;

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

        public async Task SolveLoanRequest(LoanRequest loanRequest)
        {
            await loanRequestRepository.SolveLoanRequest(loanRequest);
        }

        public  async Task DeleteLoanRequest(LoanRequest loanRequest)
        {
            await loanRequestRepository.DeleteLoanRequest(loanRequest);
        }

        public async Task<List<LoanRequest>> GetLoanRequests()
        {
            return await loanRequestRepository.GetLoanRequests();
        }

        public async Task<List<LoanRequest>> GetUnsolvedLoanRequests()
        {
            return await loanRequestRepository.GetUnsolvedLoanRequests();
        }
    }
}