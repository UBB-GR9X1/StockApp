namespace StockApp.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Repositories;
    using StockApp.Services;

    internal class CreateProfilePageViewModel : INotifyPropertyChanged
    {
        private static readonly UserService userService = new(new UserRepository());

        private string image = string.Empty;
        private string username = string.Empty;
        private string description = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string email = string.Empty;
        private string phoneNumber = string.Empty;
        private DateOnly birthday = new();
        private string cnp = string.Empty;
        private string zodiacSign = string.Empty;
        private string zodiacAttribute = string.Empty;

        public ICommand CreateProfileCommand { get; set; }

        public CreateProfilePageViewModel()
        {
            this.CreateProfileCommand = new RelayCommand(this.CreateProfile);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Image
        {
            get => this.image;
            set => this.SetProperty(ref this.image, value);
        }

        public string Username
        {
            get => this.username;
            set => this.SetProperty(ref this.username, value);
        }

        public string Description
        {
            get => this.description;
            set => this.SetProperty(ref this.description, value);
        }

        public string FirstName
        {
            get => this.firstName;
            set => this.SetProperty(ref this.firstName, value);
        }

        public string LastName
        {
            get => this.lastName;
            set => this.SetProperty(ref this.lastName, value);
        }

        public string Email
        {
            get => this.email;
            set => this.SetProperty(ref this.email, value);
        }

        public string PhoneNumber
        {
            get => this.phoneNumber;
            set => this.SetProperty(ref this.phoneNumber, value);
        }

        public DateOnly Birthday
        {
            get => this.birthday;
            set => this.SetProperty(ref this.birthday, value);
        }

        public string CNP
        {
            get => this.cnp;
            set => this.SetProperty(ref this.cnp, value);
        }

        public string ZodiacSign
        {
            get => this.zodiacSign;
            set => this.SetProperty(ref this.zodiacSign, value);
        }

        public string ZodiacAttribute
        {
            get => this.zodiacAttribute;
            set => this.SetProperty(ref this.zodiacAttribute, value);
        }

        private async void CreateProfile(object o)
        {
            User user = new()
            {
                Image = this.Image,
                Username = this.Username,
                Description = this.Description,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                Birthday = this.Birthday,
                CNP = this.CNP,
                ZodiacSign = this.ZodiacSign,
                ZodiacAttribute = this.ZodiacAttribute,
                Balance = 0,
                IsHidden = false,
                IsModerator = false,
                GemBalance = 0,
                NumberOfOffenses = 0,
            };

            await userService.CreateUser(user);
        }

        private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
