using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using StockApp.Model;

namespace StockApp.Repository.Repository.Exporter
{
    public class JSONTransactionExporter : ITransactionExporter
    {
        public void Export(List<TransactionLogTransaction> transactions, string filePath)
        {
            var jsonData = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }
    }
}
