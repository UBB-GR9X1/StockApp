namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;
    using StockApp.Repositories.Exporters;

    public class TransactionLogService : ITransactionLogService
    {
        private readonly ITransactionRepository transactionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogService"/> class.
        /// </summary>
        /// <param name="transactionRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TransactionLogService(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        /// <summary>
        /// Retrieves all transactions for the current user.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<TransactionLogTransaction> GetFilteredTransactions(TransactionFilterCriteria criteria)
        {
            criteria.Validate();
            return transactionRepository.GetByFilterCriteria(criteria);
        }

        /// <summary>
        /// Sorts a list of transactions based on the specified criteria.
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="sortType"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        /// <exception cref="InvalidSortTypeException"></exception>
        public List<TransactionLogTransaction> SortTransactions(List<TransactionLogTransaction> transactions, string sortType = "Date", bool ascending = true)
        {
            return sortType switch
            {
                "Date" => ascending
                    ? transactions.OrderBy(t => t.Date).ToList()
                    : transactions.OrderByDescending(t => t.Date).ToList(),

                "Stock Name" => ascending
                    ? transactions.OrderBy(t => t.StockName).ToList()
                    : transactions.OrderByDescending(t => t.StockName).ToList(),

                "Total Value" => ascending
                    ? transactions.OrderBy(t => t.TotalValue).ToList()
                    : transactions.OrderByDescending(t => t.TotalValue).ToList(),

                _ => throw new InvalidSortTypeException(sortType),
            };
        }

        /// <summary>
        /// Exports a list of transactions to a specified file format.
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="filePath"></param>
        /// <param name="format"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ExportFormatNotSupportedException"></exception>
        public void ExportTransactions(List<TransactionLogTransaction> transactions, string filePath, string format)
        {
            if (transactions == null)
            {
                throw new ArgumentNullException(nameof(transactions));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException("Export format is required.", nameof(format));
            }

            ITransactionExporter exporter = format.ToLower() switch
            {
                "csv" => new CSVTransactionExporter(),
                "json" => new JSONTransactionExporter(),
                "html" => new HTMLTransactionExporter(),
                _ => throw new ExportFormatNotSupportedException(format),
            };

            exporter.Export(transactions, filePath);
        }
    }
}
