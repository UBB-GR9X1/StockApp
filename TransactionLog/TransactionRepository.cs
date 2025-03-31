using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace TransactionLog
{
    public class TransactionRepository
    {
        private readonly List<Transaction> transactions;

        public TransactionRepository()
        {
            transactions = new List<Transaction>
            {
                // Sample data with various transactions
                new Transaction("AAPL", "Apple", "BUY", 10, 150, new DateTime(2023, 03, 15), "John Doe"),
                new Transaction("GOOG", "Google", "SELL", 5, 2500, new DateTime(2023, 03, 16), "Jane Smith"),
                new Transaction("AMZN", "Amazon", "BUY", 3, 3200, new DateTime(2023, 03, 17), "John Doe"),
                new Transaction("AAPL", "Apple", "SELL", 7, 155, new DateTime(2023, 03, 18), "John Doe"),
                new Transaction("MSFT", "Microsoft", "BUY", 15, 200, new DateTime(2023, 03, 19), "Jane Smith"),
                new Transaction("GOOG", "Google", "BUY", 12, 2500, new DateTime(2023, 03, 20), "Michael Johnson")
            };
        }

        public void Add(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        public List<Transaction> GetAll()
        {
            return [.. transactions];
        }

        public List<Transaction> GetByFilterCriteria(TransactionFilterCriteria criteria)
        {
            return [.. transactions.Where(transaction => 
                (string.IsNullOrEmpty(criteria.StockName) || transaction.StockName.Equals(criteria.StockName)) &&
                (string.IsNullOrEmpty(criteria.Type) || transaction.Type.Equals(criteria.Type)) &&
                (!criteria.MinTotalValue.HasValue || transaction.TotalValue >= criteria.MinTotalValue) &&
                (!criteria.MaxTotalValue.HasValue || transaction.TotalValue <= criteria.MaxTotalValue) &&
                (!criteria.StartDate.HasValue || transaction.Date >= criteria.StartDate) &&
                (!criteria.EndDate.HasValue || transaction.Date <= criteria.EndDate)
            )];
        }
    }
}
