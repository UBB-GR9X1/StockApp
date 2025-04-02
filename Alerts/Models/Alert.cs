// Models/Alert.cs
namespace Alerts.Models
{
    public class Alert
    {
        public int AlertId { get; set; }  // Changed from AlertID to AlertId
        public string Name { get; set; }
        public bool ToggleOnOff { get; set; }
        public int UpperBound { get; set; }
        public int LowerBound { get; set; }
    }
}
    