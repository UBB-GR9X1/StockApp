namespace StockApp.Models
{
    using System;

    public class Loan
    {
        public int Id { get; set; }

        public string UserCnp { get; set; }

        public decimal LoanAmount { get; set; }

        public DateTime ApplicationDate { get; set; }

        public DateTime RepaymentDate { get; set; }

        public decimal InterestRate { get; set; }

        public int NumberOfMonths { get; set; }

        public decimal MonthlyPaymentAmount { get; set; }

        public string Status { get; set; }

        public int MonthlyPaymentsCompleted { get; set; }

        public decimal RepaidAmount { get; set; }

        public decimal Penalty { get; set; }

        public Loan() { }

        public Loan(string userCnp, decimal loanAmount, DateTime applicationDate, DateTime repaymentDate, decimal interestRate, int numberOfMonths, decimal monthlyPaymentAmount, int monthlyPaymentsCompleted, decimal repaidAmount, decimal penalty, string status)
        {
            this.UserCnp = userCnp;
            this.LoanAmount = loanAmount;
            this.ApplicationDate = applicationDate;
            this.RepaymentDate = repaymentDate;
            this.InterestRate = interestRate;
            this.NumberOfMonths = numberOfMonths;
            this.MonthlyPaymentAmount = monthlyPaymentAmount;
            this.Status = status;
            this.MonthlyPaymentsCompleted = monthlyPaymentsCompleted;
            this.RepaidAmount = repaidAmount;
            this.Penalty = penalty;
        }
    }
}

