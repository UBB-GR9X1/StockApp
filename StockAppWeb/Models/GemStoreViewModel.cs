using Common.Models;

public class GemStoreViewModel
{
    public List<GemDeal> AvailableDeals { get; set; } = new();
    public List<string> UserBankAccounts { get; set; } = new();
    public int UserGems { get; set; }
    public string? SelectedBankAccount { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool IsLoading { get; set; }
    public bool IsGuest { get; set; }
    public int GemsToSell { get; set; }

    // Simulate loading data (replace with real logic as needed)
    public void Initialize()
    {
        // Example static data for demonstration
        AvailableDeals = new List<GemDeal>
        {
            new GemDeal("Starter Pack", 100, 1.99),
            new GemDeal("Pro Pack", 500, 8.99, true, 60)
        };
        UserBankAccounts = new List<string> { "Account1", "Account2" };
        UserGems = 250;
        SelectedBankAccount ??= UserBankAccounts.FirstOrDefault();
        IsGuest = false;
        GemsToSell = 0;
        ErrorMessage = null;
        SuccessMessage = null;
    }

    public string BuyGems(GemDeal deal, string selectedBankAccount)
    {
        // Simulate a successful purchase
        UserGems += deal.GemAmount;
        return $"Successfully bought {deal.GemAmount} gems!";
    }

    public string SellGems(int gemsToSell, string selectedBankAccount)
    {
        // Simulate a successful sale
        UserGems -= gemsToSell;
        return $"Successfully sold {gemsToSell} gems!";
    }
}

