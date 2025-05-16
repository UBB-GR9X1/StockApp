namespace StockApp.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class GivenTip : INotifyPropertyChanged
    {
        private int id;
        private string userCnp = string.Empty;
        private DateTime date;
        private Tip tip;

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

        public string UserCnp
        {
            get => this.userCnp;
            set
            {
                if (this.userCnp != value)
                {
                    this.userCnp = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public DateTime Date
        {
            get => this.date;
            set
            {
                if (this.date != value)
                {
                    this.date = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Tip Tip
        {
            get => this.tip;
            set
            {
                if (this.tip != value)
                {
                    this.tip = value;
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
