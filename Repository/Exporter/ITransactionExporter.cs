﻿namespace StockApp.Repository.Exporter
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface ITransactionExporter
    {
        void Export(IReadOnlyList<ITransactionLogTransaction> transactions, string filePath);
    }
}
