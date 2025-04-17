namespace StockApp.Repository.Exporter
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface ITransactionExporter
    {
        void Export(List<TransactionLogTransaction> transactions, string filePath);
    }
}
