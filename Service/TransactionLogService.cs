using System;
using System.Collections.Generic;
using System.Linq;
using StockApp.Model;
using StockApp.Repository;

namespace StockApp.Service
{
    public class TransactionLogService
    {
        private readonly TransactionRepository transactionRepository;

        public TransactionLogService(TransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public List<TransactionLogTransaction> GetFilteredTransactions(TransactionFilterCriteria criteria)
        {
            criteria.Validate();
            return transactionRepository.GetByFilterCriteria(criteria);
        }

        public List<TransactionLogTransaction> SortTransactions(List<TransactionLogTransaction> transactions, string sortType = "Date", bool ascending = true)
        {
            switch (sortType)
            {
                case "Date":
                    return ascending
                        ? [.. transactions.OrderBy(transaction => transaction.Date)]
                        : [.. transactions.OrderByDescending(transaction => transaction.Date)];
                case "Stock Name":
                    return ascending
                        ? [.. transactions.OrderBy(transaction => transaction.StockName)]
                        : [.. transactions.OrderByDescending(transaction => transaction.StockName)];
                case "Total Value":
                    return ascending
                        ? [.. transactions.OrderBy(transaction => transaction.TotalValue)]
                        : [.. transactions.OrderByDescending(transaction => transaction.TotalValue)];
                default:
                    throw new Exception("Invalid sorting type!");
            }
        }

        public void ExportTransactions(List<TransactionLogTransaction> transactions, string filePath, string format)
        {
            Repository.Repository.Exporter.ITransactionExporter exporter = format.ToLower() switch
            {
                "csv" => new Repository.Repository.Exporter.CSVTransactionExporter(),
                "json" => new Repository.Repository.Exporter.JSONTransactionExporter(),
                "html" => new Repository.Repository.Exporter.HTMLTransactionExporter(),
                _ => throw new ArgumentException("Unsupported file format."),
            };

            exporter.Export(transactions, filePath);
        }
    }
}
