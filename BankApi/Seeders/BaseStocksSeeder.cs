using BankApi.Data;
using Common.Models; // Assuming BaseStock and Stock models are in Common.Models

namespace BankApi.Seeders
{
    public class BaseStocksSeeder(IConfiguration configuration, IServiceProvider serviceProvider) : RegularTableSeeder<BaseStock>(configuration, serviceProvider)
    {
        protected override async Task SeedDataAsync(ApiDbContext context)
        {
            // Since Stock inherits from BaseStock and EF Core uses a TPH (Table Per Hierarchy) strategy by default,
            // we should seed Stock entities, and the BaseStock properties will be saved in the same table.
            // If you only want to seed BaseStock without the Stock-specific properties (Price, Quantity),
            // you would need a different approach or ensure your model supports that.
            // For TPH, Discriminator column is handled by EF Core automatically based on the concrete type being saved.

            if (context.Stocks.Any()) // Check Stocks table as it represents the concrete type
            {
                Console.WriteLine("Stocks (BaseStocks) already exist, skipping seeding.");
                return;
            }

            var stocksToSeed = new[]
            {
                // The Discriminator field is handled by EF Core; you provide the concrete type (Stock)
                // Price and Quantity are part of the Stock derived type
                new Stock { Name = "Apple Inc.", Symbol = "AAPL", AuthorCNP = "1234567890123", Price = 175, Quantity = 100 },
                new Stock { Name = "Alphabet Inc.", Symbol = "GOOGL", AuthorCNP = "9876543210987", Price = 2850, Quantity = 50 },
                new Stock { Name = "Tesla Inc.", Symbol = "TSLA", AuthorCNP = "2345678901234", Price = 725, Quantity = 200 },
                new Stock { Name = "Amazon.com Inc.", Symbol = "AMZN", AuthorCNP = "3456789012345", Price = 3425, Quantity = 80 },
                new Stock { Name = "Microsoft Corp.", Symbol = "MSFT", AuthorCNP = "4567890123456", Price = 325, Quantity = 150 }
            };

            await context.Stocks.AddRangeAsync(stocksToSeed);
        }
    }
}