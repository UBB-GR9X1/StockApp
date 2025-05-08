﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;
namespace StockApp.Repositories
{
    /// <summary>
    /// Interface for managing transactions in the application.
    /// Provides methods for retrieving, filtering, and adding transactions.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets the list of all transaction logs asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all <see cref="TransactionLogTransaction"/> objects.</returns>
        Task<List<TransactionLogTransaction>> getAllTransactions();

        /// <summary>
        /// Retrieves a list of transactions that match the specified filter criteria.
        /// </summary>
        /// <param name="criteria">The filter criteria to apply, including stock name, type, date range, and value range.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="TransactionLogTransaction"/> objects matching the criteria.</returns>
        Task<List<TransactionLogTransaction>> GetByFilterCriteria(TransactionFilterCriteria criteria);

        /// <summary>
        /// Adds a new transaction to the repository.
        /// </summary>
        /// <param name="transaction">The transaction to add, including details such as stock symbol, type, amount, and author.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddTransaction(TransactionLogTransaction transaction);
    }
}

