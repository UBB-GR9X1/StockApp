namespace BankApi.Seeders
{
    public class HomepageStocksSeeder(IConfiguration configuration) : BaseSeeder(configuration)
    {
        protected override string GetQuery() => @"
            IF NOT EXISTS (SELECT 1 FROM HomepageStocks) 
            BEGIN
                INSERT INTO HomepageStocks 
                    (Id, Symbol, Change)
                VALUES
                    (2, 'AAPL', 1.25),
                    (3, 'GOOGL', -0.75),
                    (4, 'TSLA', 2.30),
                    (5, 'AMZN', -1.10),
                    (6, 'MSFT', 0.50);
            END;
        ";
    }
}