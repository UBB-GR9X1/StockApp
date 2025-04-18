namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using Microsoft.UI.Xaml;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Pages;
    using StockApp.Services;

    public class HomepageViewModel : INotifyPropertyChanged
    {
        private readonly HomepageService service = new();

        private ObservableCollection<HomepageStock> filteredAllStocks = [];
        private ObservableCollection<HomepageStock> filteredFavoriteStocks = [];
        private string searchQuery = string.Empty;
        private string selectedSortOption = string.Empty;
        private bool isGuestUser;
        private Visibility guestButtonVisibility = Visibility.Visible;
        private Visibility profileButtonVisibility = Visibility.Collapsed;

        public HomepageViewModel()
        {
            this.IsGuestUser = this.service.IsGuestUser();
            this.LoadStocks();

            // Initialize Commands
            this.FavoriteCommand = new RelayCommand(obj => this.ToggleFavorite(obj as HomepageStock));
            this.CreateProfileCommand = new RelayCommand(_ => this.CreateUserProfile());
            this.NavigateCommand = new RelayCommand(param => this.NavigateToPage(param));
            this.SearchCommand = new RelayCommand(_ => this.ApplyFilter());
            this.SortCommand = new RelayCommand(_ => this.ApplySort());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand FavoriteCommand { get; }

        public ICommand CreateProfileCommand { get; }

        public ICommand NavigateCommand { get; }

        public ICommand SearchCommand { get; }

        public ICommand SortCommand { get; }
        public ObservableCollection<HomepageStock> FilteredAllStocks
        {
            get => this.filteredAllStocks;
            private set
            {
                this.filteredAllStocks = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<HomepageStock> FilteredFavoriteStocks
        {
            get => this.filteredFavoriteStocks;
            private set
            {
                this.filteredFavoriteStocks = value;
                this.OnPropertyChanged();
            }
        }

        public string GetUserCNP => this.service.GetUserCNP();

        public bool IsGuestUser
        {
            get => this.isGuestUser;
            set
            {
                this.isGuestUser = value;
                this.GuestButtonVisibility = this.isGuestUser ? Visibility.Visible : Visibility.Collapsed;
                this.ProfileButtonVisibility = this.isGuestUser ? Visibility.Collapsed : Visibility.Visible;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CanModifyFavorites));
            }
        }

        public Visibility GuestButtonVisibility
        {
            get => this.guestButtonVisibility;
            set
            {
                this.guestButtonVisibility = value;
                this.OnPropertyChanged();
            }
        }

        public Visibility ProfileButtonVisibility
        {
            get => this.profileButtonVisibility;
            set
            {
                this.profileButtonVisibility = value;
                this.OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => this.searchQuery;
            set
            {
                this.searchQuery = value;
                this.OnPropertyChanged();
                this.ApplyFilter();
            }
        }

        public string SelectedSortOption
        {
            get => this.selectedSortOption;
            set
            {
                this.selectedSortOption = value;
                this.OnPropertyChanged();
                this.ApplySort();
            }
        }

        public bool CanModifyFavorites => !this.isGuestUser;

        private void LoadStocks()
        {
            this.FilteredAllStocks = [.. this.service.GetAllStocks()];
            this.FilteredFavoriteStocks = [.. this.service.GetFavoriteStocks()];
        }

        public void ApplyFilter()
        {
            this.service.FilterStocks(this.SearchQuery);
            this.LoadStocks();
        }

        public void ApplySort()
        {
            this.service.SortStocks(this.SelectedSortOption);
            this.LoadStocks();
        }

        public void CreateUserProfile()
        {
            this.service.CreateUserProfile();
            this.IsGuestUser = false;
            this.LoadStocks();
        }

        public void ToggleFavorite(HomepageStock stock)
        {
            if (stock == null)
            {
                return;
            }

            if (stock.IsFavorite)
            {
                this.service.RemoveFromFavorites(stock);
            }
            else
            {
                this.service.AddToFavorites(stock);
            }

            this.LoadStocks();
        }

        public void NavigateToPage(object parameter)
        {
            if (parameter is string pageName)
            {
                switch (pageName)
                {
                    case "NewsListPage":
                        NavigationService.Instance.Navigate(typeof(NewsListPage), parameter);
                        break;
                    case "CreateStockPage":
                        NavigationService.Instance.Navigate(typeof(CreateStockPage), parameter);
                        break;
                    case "TransactionLogPage":
                        NavigationService.Instance.Navigate(typeof(TransactionLogPage), parameter);
                        break;
                    case "ProfilePage":
                        NavigationService.Instance.Navigate(typeof(ProfilePage), parameter);
                        break;
                    case "GemStoreWindow":
                        NavigationService.Instance.Navigate(typeof(GemStoreWindow), parameter);
                        break;
                    default:
                        throw new ArgumentException($"Unknown page: {pageName}", nameof(pageName));
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}