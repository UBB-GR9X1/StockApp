using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

    public class Tip : INotifyPropertyChanged
    {
        private int id;
        private string creditScoreBracket = string.Empty;
        private string tipText = string.Empty;

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
        [MaxLength(100)]
        public string CreditScoreBracket
        {
            get => this.creditScoreBracket;
            set
            {
                if (this.creditScoreBracket != value)
                {
                    this.creditScoreBracket = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required]
        [MaxLength(1000)]
        public string TipText
        {
            get => this.tipText;
            set
            {
                if (this.tipText != value)
                {
                    this.tipText = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
