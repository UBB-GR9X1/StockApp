using System.Collections.Generic;
using StockApp.Model;

namespace StockApp.Repository.Repository.Exporter
{
    public interface ITransactionExporter
    {
        void Export(List<TransactionLogTransaction> transactions, string filePath);
    }
}
