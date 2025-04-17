namespace StockApp.Models
{
    using System;

    public class TransactionFilterCriteria : ITransactionFilterCriteria
    {
        public string? StockName { get; set; }

        public string? Type { get; set; }

        public int? MinTotalValue { get; set; }

        public int? MaxTotalValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public void Validate()
        {
            if (!string.IsNullOrEmpty(Type)
                && Type is not ("BUY" or "SELL"))
            {
                throw new ArgumentException(
                    "Type must be \"BUY\" or \"SELL\".");
            }

            if (MinTotalValue.HasValue
                && MaxTotalValue.HasValue
                && MinTotalValue > MaxTotalValue)
            {
                throw new ArgumentException(
                    "Min cannot exceed Max total value.");
            }

            if (StartDate.HasValue
                && EndDate.HasValue
                && StartDate > EndDate)
            {
                throw new ArgumentException(
                    "StartDate cannot be after EndDate.");
            }
        }
    }
}
