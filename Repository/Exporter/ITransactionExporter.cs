using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockApp.Model;

namespace StockApp.Repository.Repository.Exporter
{
    interface ITransactionExporter
    {
        void Export(List<TransactionLogTransaction> transactions, string filePath);
    }
}
