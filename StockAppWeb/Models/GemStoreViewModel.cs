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
}
