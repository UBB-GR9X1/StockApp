namespace BankApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BankApi.Repositories;
    using Common.Models;
    using Common.Services;

    public class LoanService(ILoanRepository loanRepository, IUserRepository userRepository) : ILoanService
    {
        private readonly ILoanRepository loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
        private readonly IUserRepository userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        public async Task<List<Loan>> GetLoansAsync()
        {
            return await loanRepository.GetLoansAsync();
        }

        public async Task<List<Loan>> GetUserLoansAsync(string userCNP)
        {
            return await loanRepository.GetUserLoansAsync(userCNP);
        }

        public async Task AddLoanAsync(LoanRequest loanRequest)
        {
            User user = await userRepository.GetByCnpAsync(loanRequest.UserCnp) ?? throw new Exception("User not found");

            decimal interestRate = (decimal)user.RiskScore / user.CreditScore * 100;
            int noMonths = (loanRequest.RepaymentDate.Year - loanRequest.ApplicationDate.Year) * 12 + loanRequest.RepaymentDate.Month - loanRequest.ApplicationDate.Month;
            decimal monthlyPaymentAmount = loanRequest.Amount * ((1 + interestRate / 100) / noMonths);
            int monthlyPaymentsCompleted = 0;
            int repaidAmount = 0;
            decimal penalty = 0;

            Loan loan = new()
            {
                UserCnp = loanRequest.UserCnp,
                LoanAmount = loanRequest.Amount,
                ApplicationDate = loanRequest.ApplicationDate,
                RepaymentDate = loanRequest.RepaymentDate,
                MonthlyPaymentAmount = monthlyPaymentAmount,
                MonthlyPaymentsCompleted = monthlyPaymentsCompleted,
                NumberOfMonths = noMonths,
                Status = "active",
                Penalty = penalty,
                RepaidAmount = repaidAmount,
                InterestRate = interestRate,
            };

            await loanRepository.AddLoanAsync(loan);
        }

        public async Task CheckLoansAsync()
        {
            List<Loan> loanList = await loanRepository.GetLoansAsync();
            foreach (Loan loan in loanList)
            {
                int numberOfMonthsPassed = (DateTime.Today.Year - loan.ApplicationDate.Year) * 12 + DateTime.Today.Month - loan.ApplicationDate.Month;
                User user = await userRepository.GetByCnpAsync(loan.UserCnp) ?? throw new Exception("User not found");
                if (loan.MonthlyPaymentsCompleted >= loan.NumberOfMonths)
                {
                    loan.Status = "completed";
                    int newUserCreditScore = ILoanService.ComputeNewCreditScore(user, loan);

                    user.CreditScore = newUserCreditScore;
                    await userRepository.UpdateAsync(user);
                }

                if (numberOfMonthsPassed > loan.MonthlyPaymentsCompleted)
                {
                    int numberOfOverdueDays = (DateTime.Today - loan.ApplicationDate.AddMonths(loan.MonthlyPaymentsCompleted)).Days;
                    decimal penalty = (decimal)(0.1 * numberOfOverdueDays);
                    loan.Penalty = penalty;
                }
                else
                {
                    loan.Penalty = 0;
                }

                if (DateTime.Today > loan.RepaymentDate && loan.Status == "active")
                {
                    loan.Status = "overdue";
                    int newUserCreditScore = ILoanService.ComputeNewCreditScore(user, loan);

                    user.CreditScore = newUserCreditScore;
                    await userRepository.UpdateAsync(user);
                    await UpdateHistoryForUserAsync(loan.UserCnp, newUserCreditScore);
                }
                else if (loan.Status == "overdue")
                {
                    if (loan.MonthlyPaymentsCompleted >= loan.NumberOfMonths)
                    {
                        loan.Status = "completed";
                        int newUserCreditScore = ILoanService.ComputeNewCreditScore(user, loan);

                        user.CreditScore = newUserCreditScore;
                        await userRepository.UpdateAsync(user);
                        await UpdateHistoryForUserAsync(loan.UserCnp, newUserCreditScore);
                    }
                }

                if (loan.Status == "completed")
                {
                    await loanRepository.DeleteLoanAsync(loan.Id);
                }
                else
                {
                    await loanRepository.UpdateLoanAsync(loan);
                }
            }
        }

        public async Task UpdateHistoryForUserAsync(string userCNP, int newScore)
        {
            await loanRepository.UpdateCreditScoreHistoryForUserAsync(userCNP, newScore);
        }

        public async Task IncrementMonthlyPaymentsCompletedAsync(int loanID, decimal penalty)
        {
            Loan loan = await loanRepository.GetLoanByIdAsync(loanID);
            loan.MonthlyPaymentsCompleted++;
            loan.RepaidAmount += loan.MonthlyPaymentAmount + penalty;
            await loanRepository.UpdateLoanAsync(loan);
        }
    }
}
