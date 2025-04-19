namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using Catel.Services;
    using Microsoft.UI.Xaml;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Pages;
    using StockApp.Services;
    using NavigationService = Services.NavigationService;


    public class HomepageViewModel : INotifyPropertyChanged
    {
        private readonly IHomepageService Service;

        private ObservableCollection<HomepageStock> FilteredAllStocks = [];
        private ObservableCollection<HomepageStock> FilteredFavoriteStocks = [];
        private string SearchQuery = string.Empty;
        private string SelectedSortOption = string.Empty;
        private bool IsGuestUser;
        private Visibility GuestButtonVisibility = Visibility.Visible;
        private Visibility ProfileButtonVisibility = Visibility.Collapsed;

        public HomepageViewModel(IHomepageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));

            this.IsGuestUser = this.Service.IsGuestUser();
            this.LoadStocks();

            // Initialize Commands
            this.FavoriteCommand = new RelayCommand(obj => this.ToggleFavorite(obj as HomepageStock));
            this.CreateProfileCommand = new RelayCommand(_ => this.CreateUserProfile());
            this.NavigateCommand = new RelayCommand(param => this.NavigateToPage(param));
            this.SearchCommand = new RelayCommand(_ => this.ApplyFilter());
            this.SortCommand = new RelayCommand(_ => this.ApplySort());
        }

        public HomepageViewModel()
          : this(new HomepageService()) { }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand FavoriteCommand { get; }

        public ICommand CreateProfileCommand { get; }

        public ICommand NavigateCommand { get; }

        public ICommand SearchCommand { get; }

        public ICommand SortCommand { get; }
        public ObservableCollection<HomepageStock> FilteredAllStocks
        {
            get => this.FilteredAllStocks;
            private set
            {
                this.FilteredAllStocks = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<HomepageStock> FilteredFavoriteStocks
        {
            get => this.FilteredFavoriteStocks;
            private set
            {
                this.FilteredFavoriteStocks = value;
                this.OnPropertyChanged();
            }
        }

        public string GetUserCNP => this.Service.GetUserCNP();

        public bool IsGuestUser
        {
            get => this.IsGuestUser;
            set
            {
                this.IsGuestUser = value;
                this.GuestButtonVisibility = this.IsGuestUser ? Visibility.Visible : Visibility.Collapsed;
                this.ProfileButtonVisibility = this.IsGuestUser ? Visibility.Collapsed : Visibility.Visible;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CanModifyFavorites));
            }
        }

        public Visibility GuestButtonVisibility
        {
            get => this.GuestButtonVisibility;
            set
            {
                this.GuestButtonVisibility = value;
                this.OnPropertyChanged();
            }
        }

        public Visibility ProfileButtonVisibility
        {
            get => this.ProfileButtonVisibility;
            set
            {
                this.ProfileButtonVisibility = value;
                this.OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => this.SearchQuery;
            set
            {
                this.SearchQuery = value;
                this.OnPropertyChanged();
                this.ApplyFilter();
            }
        }

        public string SelectedSortOption
        {
            get => this.SelectedSortOption;
            set
            {
                this.SelectedSortOption = value;
                this.OnPropertyChanged();
                this.ApplySort();
            }
        }

        public bool CanModifyFavorites => !this.IsGuestUser;

        private void LoadStocks()
        {
            this.FilteredAllStocks = [.. this.Service.GetAllStocks()];
            this.FilteredFavoriteStocks = [.. this.Service.GetFavoriteStocks()];
        }

        public void ApplyFilter()
        {
            this.Service.FilterStocks(this.SearchQuery);
            this.LoadStocks();
        }

        public void ApplySort()
        {
            this.Service.SortStocks(this.SelectedSortOption);
            this.LoadStocks();
        }

        public void CreateUserProfile()
        {
            this.Service.CreateUserProfile();
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
                this.Service.RemoveFromFavorites(stock);
            }
            else
            {
                this.Service.AddToFavorites(stock);
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