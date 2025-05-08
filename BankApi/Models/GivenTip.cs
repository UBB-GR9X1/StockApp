
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a tip that has been marked as given to another user.
    /// </summary>
    public class GivenTip
    {
        private int id;
        private int tipId;
        private string givenToUser = string.Empty;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the unique identifier for this given tip.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ID of the tip being given.
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
        /// Gets or sets the name of the user who received the tip.
        /// </summary>
        public string GivenToUser
        {
            get => this.givenToUser;
            set
            {
                if (this.givenToUser != value)
                {
                    this.givenToUser = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }