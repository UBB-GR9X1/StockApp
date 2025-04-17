// return new global::StocksHomepage.Model.Stock { Change="", isFavorite=false, Name="", Price="", Symbol="" };

namespace StockApp.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using StockApp.Models;
    using StockApp.Service;

    public class HomepageViewModel : INotifyPropertyChanged
    {
        private readonly HomepageService service;
        private ObservableCollection<HomepageStock> filteredAllStocks;
        private ObservableCollection<HomepageStock> filteredFavoriteStocks;
        private string searchQuery;
        private string selectedSortOption;
        private bool isGuestUser = true;
        private string guestButtonVisibility = "Visible";
        private string profileButtonVisibility = "Collapsed";

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand FavoriteCommand { get; }

        public ObservableCollection<HomepageStock> FilteredAllStocks
        {
            get => this.filteredAllStocks;
            private set
            {
                this.filteredAllStocks = value;
                this.OnPropertyChanged(nameof(this.FilteredAllStocks));
            }
        }

        public ObservableCollection<HomepageStock> FilteredFavoriteStocks
        {
            get => this.filteredFavoriteStocks;
            private set
            {
                this.filteredFavoriteStocks = value;
                this.OnPropertyChanged(nameof(this.FilteredFavoriteStocks));
            }
        }

        public string GetUserCNP() => this.service.GetUserCNP();

        public bool IsGuestUser
        {
            get => this.isGuestUser;
            set
            {
                this.isGuestUser = value;
                this.GuestButtonVisibility = this.isGuestUser ? "Visible" : "Collapsed";
                this.ProfileButtonVisibility = this.isGuestUser ? "Collapsed" : "Visible";
                this.OnPropertyChanged(nameof(this.IsGuestUser));
                this.OnPropertyChanged(nameof(this.CanModifyFavorites)); // Add this line
            }
        }

        public string GuestButtonVisibility
        {
            get => this.guestButtonVisibility;
            set
            {
                this.guestButtonVisibility = value;
                this.OnPropertyChanged(nameof(this.GuestButtonVisibility));
            }
        }

        public string ProfileButtonVisibility
        {
            get => this.profileButtonVisibility;
            set
            {
                this.profileButtonVisibility = value;
                this.OnPropertyChanged(nameof(this.ProfileButtonVisibility));
            }
        }

        public string SearchQuery
        {
            get => this.searchQuery;
            set
            {
                this.searchQuery = value;
                this.OnPropertyChanged(nameof(this.SearchQuery));
                this.ApplyFilter();
            }
        }

        public string SelectedSortOption
        {
            get => this.selectedSortOption;
            set
            {
                this.selectedSortOption = value;
                this.OnPropertyChanged(nameof(this.SelectedSortOption));
                this.ApplySort();
            }
        }

        public HomepageViewModel()
        {
            this.service = new HomepageService();
            this.IsGuestUser = this.service.IsGuestUser();
            this.FilteredAllStocks = [.. this.service.GetAllStocks()];
            this.FilteredFavoriteStocks = [.. this.service.GetFavoriteStocks()];
            this.FavoriteCommand = new RelayCommand(obj => this.ToggleFavorite(obj as HomepageStock), this.CanToggleFavorite);

            //FavoriteCommand = new RelayCommand(ToggleFavorite, CanToggleFavorite);
        }

        public bool CanModifyFavorites
        {
            get => !this.isGuestUser;
        }

        public bool CanToggleFavorite(object obj) => !this.IsGuestUser;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ApplyFilter()
        {
            this.service.FilterStocks(this.SearchQuery);
            this.FilteredAllStocks = this.service.FilteredAllStocks;
            this.FilteredFavoriteStocks = this.service.FilteredFavoriteStocks;
        }

        public void CreateUserProfile()
        {
            // Call the service to create a user profile
            this.service.CreateUserProfile();

            // Update the guest status
            this.IsGuestUser = false;

            // Refresh the stocks to reflect new permissions
            this.RefreshStocks();
        }

        public void ApplySort()
        {
            this.service.SortStocks(this.SelectedSortOption);
            this.FilteredAllStocks = this.service.FilteredAllStocks;
            this.FilteredFavoriteStocks = this.service.FilteredFavoriteStocks;
        }

        public void ToggleFavorite(HomepageStock stock)
        {
            if (stock.IsFavorite)
            {
                this.service.RemoveFromFavorites(stock);
                this.RefreshStocks();
                return;
            }

            this.service.AddToFavorites(stock);
            this.RefreshStocks();
        }

        public void RefreshStocks()
        {
            this.FilteredAllStocks = [.. this.service.GetAllStocks()];
            this.FilteredFavoriteStocks = [.. this.service.GetFavoriteStocks()];
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object> execute;
            private readonly Func<object, bool> canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
            {
                this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
                this.canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter) => this.canExecute?.Invoke(parameter) ?? true;

            public void Execute(object parameter) => this.execute(parameter);
        }
    }
}
