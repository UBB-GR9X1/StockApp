namespace BankApi.Seeders
{
    public class TransactionLogTransactionsSeeder(IConfiguration configuration) : BaseSeeder(configuration)
    {
        protected override string GetQuery() => @"
            IF NOT EXISTS (SELECT 1 FROM TransactionLogTransactions) 
            BEGIN
                INSERT INTO TransactionLogTransactions 
                    (Id, StockSymbol, StockName, Type, Amount, PricePerStock, Date)
                VALUES
                    (21, 'AAPL', 'Apple Inc.', 'BUY', 10, 150, '2025-04-01'),
                    (22, 'GOOGL', 'Alphabet Inc.', 'SELL', 5, 2800, '2025-03-15'),
                    (23, 'TSLA', 'Tesla Inc.', 'BUY', 20, 750, '2025-02-20'),
                    (24, 'AMZN', 'Amazon.com Inc.', 'SELL', 8, 3400, '2025-01-10'),
                    (25, 'MSFT', 'Microsoft Corp.', 'BUY', 15, 320, '2025-05-05');
            END;
        ";
    }
}