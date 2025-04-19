namespace StockApp.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Alert : INotifyPropertyChanged
    {
        private int alertId;
        private string stockName = string.Empty;
        private string name = string.Empty;
        private decimal upperBound;
        private decimal lowerBound;
        private bool toggleOnOff;

        public event PropertyChangedEventHandler PropertyChanged;

        public int AlertId
        {
            get => this.alertId;
            set
            {
                if (this.alertId != value)
                {
                    this.alertId = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string StockName
        {
            get => this.stockName;
            set
            {
                if (this.stockName != value)
                {
                    this.stockName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => this.name;
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public decimal UpperBound
        {
            get => this.upperBound;
            set
            {
                if (this.upperBound != value)
                {
                    this.upperBound = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public decimal LowerBound
        {
            get => this.lowerBound;
            set
            {
                if (this.lowerBound != value)
                {
                    this.lowerBound = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool ToggleOnOff
        {
            get => this.toggleOnOff;
            set
            {
                if (this.toggleOnOff != value)
                {
                    this.toggleOnOff = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
