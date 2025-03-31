using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionLog
{
    public class TransactionLogService
    {
        private readonly TransactionRepository transactionRepository;

        public TransactionLogService(TransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public List<Transaction> GetFilteredTransactions(TransactionFilterCriteria criteria)
        {
            criteria.Validate();
            return transactionRepository.GetByFilterCriteria(criteria);
        }

        public List<Transaction> SortTransactions(List<Transaction> transactions, string sortType = "Date", bool ascending = true)
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

        public void ExportTransactions(List<Transaction> transactions, string filePath, string format)
        {
            ITransactionExporter exporter = format.ToLower() switch
            {
                "csv" => new CSVTransactionExporter(),
                "json" => new JSONTransactionExporter(),
                "html" => new HTMLTransactionExporter(),
                _ => throw new ArgumentException("Unsupported file format."),
            };

            exporter.Export(transactions, filePath);
        }
    }
}
