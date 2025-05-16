namespace StockApp.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Tip : INotifyPropertyChanged
    {
        private int id;
        private string creditScoreBracket = string.Empty;
        private string tipText = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id
        {
            get => this.id;
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string CreditScoreBracket
        {
            get => this.creditScoreBracket;
            set
            {
                if (this.creditScoreBracket != value)
                {
                    this.creditScoreBracket = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string TipText
        {
            get => this.tipText;
            set
            {
                if (this.tipText != value)
                {
                    this.tipText = value;
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
