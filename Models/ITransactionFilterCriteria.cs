namespace StockApp.Models
{
    using System;

    public interface ITransactionFilterCriteria
    {
        string? StockName { get; set; }

        string? Type { get; set; }

        int? MinTotalValue { get; set; }

        int? MaxTotalValue { get; set; }

        DateTime? StartDate { get; set; }

        DateTime? EndDate { get; set; }

        void Validate();
    }
}
