namespace StockApp.Models
{
    public class StockPageUser(string cnp, string name, int gem_balance)
    {
        public string CNP { get; set; } = cnp;

        public string Name { get; set; } = name;

        public int GemBalance { get; set; } = gem_balance;
    }
}
