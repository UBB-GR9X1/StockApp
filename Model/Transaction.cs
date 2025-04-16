using System;

namespace StockApp.Model
{
    public class Transaction : BaseStock
    {
        private string transactionType;
        private int amount;
        private int pricePerStock;
        private int totalValue;
        private DateTime transactionDate;
        private string transactionAuthorCNP;

        public Transaction(string name, string symbol, string authorCnp, string transactionType, int amount, int pricePerStock, int totalValue, DateTime transactionDate, string transactionAuthorCNP) : base(name, symbol, authorCnp)
        {
            this.transactionType = transactionType;
            this.amount = amount;
            this.pricePerStock = pricePerStock;
            this.totalValue = totalValue;
            this.transactionDate = transactionDate;
            this.transactionAuthorCNP = transactionAuthorCNP;
        }

        public string TransactionType
        {
            get { return transactionType; }
            set { transactionType = value; }
        }

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public int PricePerStock
        {
            get { return pricePerStock; }
            set { pricePerStock = value; }
        }

        public int TotalValue
        {
            get { return totalValue; }
            set { totalValue = value; }
        }

        public DateTime TransactionDate
        {
            get { return transactionDate; }
            set { transactionDate = value; }
        }

        public string TransactionAuthorCNP
        {
            get { return transactionAuthorCNP; }
            set { transactionAuthorCNP = value; }
        }
    }
}
