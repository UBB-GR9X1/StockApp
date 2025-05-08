namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface ITransactionRepository
    {
        List<TransactionLogTransaction> Transactions { get; }

        List<TransactionLogTransaction> GetByFilterCriteria(TransactionFilterCriteria criteria);

        void AddTransaction(TransactionLogTransaction transaction);

        Task<List<TransactionLogTransaction>> GetTransactionsSinceAsync(DateTime dateOfTransaction, string userId);
    }
}
