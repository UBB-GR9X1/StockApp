using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

    public class GivenTip : INotifyPropertyChanged
    {
        private int id;
        private string userCnp = string.Empty;
        private int tipID;
        private int? messageID;
        private DateOnly date;
        private Tip tip = new();

        [Key]
        public int Id
        {
            get => this.id;
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required]
        [MaxLength(13)]
        public string UserCnp
        {
            get => this.userCnp;
            set
            {
                if (this.userCnp != value)
                {
                    this.userCnp = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TipID
        {
            get => this.tipID;
            set
            {
                if (this.tipID != value)
                {
                    this.tipID = value;
                    OnPropertyChanged();
                }
            }
        }

        [ForeignKey("TipID")]
        public Tip Tip
        {
            get => this.tip;
            set
            {
                if (this.tip != value)
                {
                    this.tip = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? MessageID
        {
            get => this.messageID;
            set
            {
                if (this.messageID != value)
                {
                    this.messageID = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateOnly Date
        {
            get => this.date;
            set
            {
                if (this.date != value)
                {
                    this.date = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
