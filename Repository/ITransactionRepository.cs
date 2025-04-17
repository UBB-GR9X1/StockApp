namespace StockApp.Repository
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface ITransactionRepository
    {
        List<ITransactionLogTransaction> Transactions { get; }
        List<ITransactionLogTransaction> GetByFilterCriteria(ITransactionFilterCriteria criteria);

        void AddTransaction(ITransactionLogTransaction transaction);
    }
}
