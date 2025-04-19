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
        private readonly TransactionRepository transactionRepository;

        public TransactionLogService(TransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        public List<TransactionLogTransaction> GetFilteredTransactions(TransactionFilterCriteria criteria)
        {
            criteria.Validate();
            return transactionRepository.GetByFilterCriteria(criteria);
        }

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
