namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface ITransactionRepository
    {
        List<TransactionLogTransaction> Transactions { get; }
        List<TransactionLogTransaction> GetByFilterCriteria(TransactionFilterCriteria criteria);

        void AddTransaction(TransactionLogTransaction transaction);
    }
}
