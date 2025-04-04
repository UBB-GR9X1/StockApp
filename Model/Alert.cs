namespace Models;
public class Alert
{
    public int AlertId { get; set; } // Primary Key
    public string StockName { get; set; } // Foreign Key
    public string Name { get; set; }
    public int UpperBound { get; set; }
    public int LowerBound { get; set; }
    public bool ToggleOnOff { get; set; }
}

