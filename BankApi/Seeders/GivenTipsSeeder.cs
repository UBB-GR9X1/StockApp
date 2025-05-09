namespace BankApi.Seeders
{
    public class GivenTipsSeeder(IConfiguration configuration) : BaseSeeder(configuration, query)
    {
        private const string query = @"
            IF NOT EXISTS (SELECT 1 FROM GivenTips) 
            BEGIN
                INSERT INTO GivenTips 
                    (TipId, UserCNP, Date)
                VALUES
                    (1, '1234567890123', '2025-04-01'),
                    (2, '9876543210987', '2025-03-15'),
                    (3, '2345678901234', '2025-02-20'),
                    (4, '3456789012345', '2025-01-10'),
                    (5, '4567890123456', '2025-05-05');
            END;
        ";
    }
}