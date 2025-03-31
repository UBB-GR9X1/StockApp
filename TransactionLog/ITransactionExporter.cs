using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionLog
{
    interface ITransactionExporter
    {
        void Export(List<Transaction> transactions, string filePath);
    }
}
