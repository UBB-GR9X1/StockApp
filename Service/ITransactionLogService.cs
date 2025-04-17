namespace StockApp.Service
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface ITransactionLogService
    {
        IReadOnlyList<ITransactionLogTransaction>
            GetFilteredTransactions(ITransactionFilterCriteria criteria);

        IReadOnlyList<ITransactionLogTransaction>
            SortTransactions(
                IReadOnlyList<ITransactionLogTransaction> transactions,
                string sortType = "Date",
                bool ascending = true);

        void ExportTransactions(
            IReadOnlyList<ITransactionLogTransaction> transactions,
            string filePath,
            string format);
    }

}
