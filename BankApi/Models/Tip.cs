
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a stock tip provided by a user.
    /// </summary>
    public class Tip 
    {
        private int tipId;
        private string user = string.Empty;
        private string stockName = string.Empty;
        private string message = string.Empty;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the unique identifier for this tip.
        /// </summary>
        public int TipId
        {
            get => this.tipId;
            set
            {
                if (this.tipId != value)
                {
                    this.tipId = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the user who provided the tip.
        /// </summary>
        public string User
        {
            get => this.user;
            set
            {
                if (this.user != value)
                {
                    this.user = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the stock name related to the tip.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the message content of the tip.
        /// </summary>
        public string Message
        {
            get => this.message;
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
