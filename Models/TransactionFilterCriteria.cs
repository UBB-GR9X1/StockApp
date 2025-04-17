namespace StockApp.Models
{
    using System;

    public class TransactionFilterCriteria
    {
        public string? StockName { get; set; }

        public string? Type { get; set; }

        public int? MinTotalValue { get; set; }

        public int? MaxTotalValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public void Validate()
        {
            if (!string.IsNullOrEmpty(this.Type) && !this.Type.Equals("BUY") && !this.Type.Equals("SELL"))
            {
                throw new Exception("The type must be \"BUY\" or \"SELL\"!");
            }

            if (this.MinTotalValue.HasValue && this.MaxTotalValue.HasValue && this.MinTotalValue > this.MaxTotalValue)
            {
                throw new Exception("The min total value cannot be greater than the max total value!");
            }

            if (this.StartDate.HasValue && this.EndDate.HasValue && this.StartDate > this.EndDate)
            {
                throw new Exception("The start date cannot be chronologically after the end date!");
            }
        }
    }
}
