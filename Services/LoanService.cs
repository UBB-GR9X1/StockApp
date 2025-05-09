namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    public class LoanService : ILoanService
    {
        private readonly ILoanRepository loanRepository;
        private readonly IUserRepository userRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            this.loanRepository = loanRepository;
        }

        public List<Loan> GetLoans()
        {
            return this.loanRepository.GetLoans();
        }

        public List<Loan> GetUserLoans(string userCNP)
        {
            return this.loanRepository.GetUserLoans(userCNP);
        }

        public void AddLoan(LoanRequest loanRequest)
        {
            User user = userRepository.GetByCnpAsync(loanRequest.UserCnp).Result;

            if (user == null)
            {
                throw new Exception("User not found");
            }

            float interestRate = (float)user.RiskScore / user.CreditScore * 100;
            int noMonths = (loanRequest.RepaymentDate.Year - loanRequest.ApplicationDate.Year) * 12 + loanRequest.RepaymentDate.Month - loanRequest.ApplicationDate.Month;
            float monthlyPaymentAmount = (float)loanRequest.Amount * (1 + interestRate / 100) / noMonths;
            int monthlyPaymentsCompleted = 0;
            int repaidAmount = 0;
            float penalty = 0;

            Loan loan = new Loan(
                loanRequest.Id,
                loanRequest.UserCnp,
                loanRequest.Amount,
                loanRequest.ApplicationDate,
                loanRequest.RepaymentDate,
                interestRate,
                noMonths,
                monthlyPaymentAmount,
                monthlyPaymentsCompleted,
                repaidAmount,
                penalty,
                "active");

            this.loanRepository.AddLoan(loan);
        }

        public async void CheckLoans()
        {
            List<Loan> loanList = this.loanRepository.GetLoans();
            foreach (Loan loan in loanList)
            {
                int numberOfMonthsPassed = ((DateTime.Today.Year - loan.ApplicationDate.Year) * 12) + DateTime.Today.Month - loan.ApplicationDate.Month;
                User user = await this.userRepository.GetByCnpAsync(loan.UserCnp) ?? throw new Exception("User not found");
                if (loan.MonthlyPaymentsCompleted >= loan.NumberOfMonths)
                {
                    loan.Status = "completed";
                    int newUserCreditScore = this.ComputeNewCreditScore(user, loan);

                    user.CreditScore = newUserCreditScore;
                    await this.userRepository.UpdateAsync(user.Id, user);
                }

                if (numberOfMonthsPassed > loan.MonthlyPaymentsCompleted)
                {
                    int numberOfOverdueDays = (DateTime.Today - loan.ApplicationDate.AddMonths(loan.MonthlyPaymentsCompleted)).Days;
                    float penalty = 0.1f * numberOfOverdueDays;
                    loan.Penalty = penalty;
                }
                else
                {
                    loan.Penalty = 0;
                }

                if (DateTime.Today > loan.RepaymentDate && loan.Status == "active")
                {
                    loan.Status = "overdue";
                    int newUserCreditScore = this.ComputeNewCreditScore(user, loan);

                    user.CreditScore = newUserCreditScore;
                    await this.userRepository.UpdateAsync(user.Id, user);
                    this.UpdateHistoryForUser(loan.UserCnp, newUserCreditScore);
                }
                else if (loan.Status == "overdue")
                {
                    if (loan.MonthlyPaymentsCompleted >= loan.NumberOfMonths)
                    {
                        loan.Status = "completed";
                        int newUserCreditScore = this.ComputeNewCreditScore(user, loan);

                        user.CreditScore = newUserCreditScore;
                        await this.userRepository.UpdateAsync(user.Id, user);
                        this.UpdateHistoryForUser(loan.UserCnp, newUserCreditScore);
                    }
                }

                if (loan.Status == "completed")
                {
                    this.loanRepository.DeleteLoan(loan.Id);
                }
                else
                {
                    this.loanRepository.UpdateLoan(loan);
                }
            }
        }

        public int ComputeNewCreditScore(User user, Loan loan)
        {
            int totalDaysInAdvance = (loan.RepaymentDate - DateTime.Today).Days;
            if (totalDaysInAdvance > 30)
            {
                totalDaysInAdvance = 30;
            }
            else if (totalDaysInAdvance < -100)
            {
                totalDaysInAdvance = -100;
            }

            int newUserCreditScore = user.CreditScore + (int)loan.LoanAmount * 10 / user.Income + totalDaysInAdvance;
            newUserCreditScore = Math.Min(newUserCreditScore, 700);
            newUserCreditScore = Math.Max(newUserCreditScore, 100);

            return newUserCreditScore;
        }

        public void UpdateHistoryForUser(string userCNP, int newScore)
        {
            this.loanRepository.UpdateCreditScoreHistoryForUser(userCNP, newScore);
        }

        public void IncrementMonthlyPaymentsCompleted(int loanID, float penalty)
        {
            Loan loan = this.loanRepository.GetLoanById(loanID);
            loan.MonthlyPaymentsCompleted++;
            loan.RepaidAmount += loan.MonthlyPaymentAmount + penalty;
            this.loanRepository.UpdateLoan(loan);
        }
    }
}
