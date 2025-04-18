namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface ITransactionLogService
    {
        List<TransactionLogTransaction>
            GetFilteredTransactions(TransactionFilterCriteria criteria);

        List<TransactionLogTransaction>
            SortTransactions(
                List<TransactionLogTransaction> transactions,
                string sortType = "Date",
                bool ascending = true);

        void ExportTransactions(
            List<TransactionLogTransaction> transactions,
            string filePath,
            string format);
    }

}
