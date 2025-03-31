using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TransactionLog
{
    public class JSONTransactionExporter : ITransactionExporter
    {
        public void Export(List<Transaction> transactions, string filePath)
        {
            var jsonData = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }
    }
}
