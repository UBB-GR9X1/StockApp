namespace BankApi.Models
{
    using System;

    public class Investment
    {
        public int Id { get; set; }

        public string InvestorCnp { get; set; }

        public string Details { get; set; }

        public decimal AmountInvested { get; set; }

        public decimal AmountReturned { get; set; }

        public DateTime InvestmentDate { get; set; }

        public Investment(int id, string investorCnp, string details, decimal amountInvested, decimal amountReturned, DateTime investmentDate)
        {
            this.Id = id;
            this.InvestorCnp = investorCnp;
            this.Details = details;
            this.AmountInvested = amountInvested;
            this.AmountReturned = amountReturned;
            this.InvestmentDate = investmentDate;
        }

        public Investment()
        {
            this.Id = 0;
            this.InvestorCnp = string.Empty;
            this.Details = string.Empty;
            this.AmountInvested = 0;
            this.AmountReturned = 0;
            this.InvestmentDate = DateTime.Now;
        }
    }
}