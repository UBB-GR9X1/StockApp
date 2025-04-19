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

    /// <summary>
    /// ViewModel for the homepage, managing stock display, filtering, sorting, and navigation.
    /// </summary>
    public class HomepageViewModel : INotifyPropertyChanged
    {
        private readonly IHomepageService service;

        // FIXME: '=[]' is invalid syntax; should initialize with 'new ObservableCollection<HomepageStock>()'
        private ObservableCollection<HomepageStock> filteredAllStocks = [];

        // FIXME: '=[]' is invalid syntax; should initialize with 'new ObservableCollection<HomepageStock>()'
        private ObservableCollection<HomepageStock> filteredFavoriteStocks = [];

        private string searchQuery = string.Empty;
        private string selectedSortOption = string.Empty;
        private bool isGuestUser = true;
        private Visibility guestButtonVisibility = Visibility.Visible;
        private Visibility profileButtonVisibility = Visibility.Collapsed;

        public HomepageViewModel(IHomepageService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));

            // setter used intentionally
            this.IsGuestUser = this.service.IsGuestUser();
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the command to toggle a stock's favorite status.
        /// </summary>
        public ICommand FavoriteCommand { get; }

        /// <summary>
        /// Gets the command to create a new user profile.
        /// </summary>
        public ICommand CreateProfileCommand { get; }

        /// <summary>
        /// Gets the command to navigate to a different page.
        /// </summary>
        public ICommand NavigateCommand { get; }

        /// <summary>
        /// Gets the command to apply the current search filter.
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// Gets the command to apply the current sort option.
        /// </summary>
        public ICommand SortCommand { get; }

        /// <summary>
        /// Gets or sets the collection of all stocks after filtering.
        /// </summary>
        public ObservableCollection<HomepageStock> FilteredAllStocks
        {
            get => this.filteredAllStocks;
            private set
            {
                this.filteredAllStocks = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the collection of favorite stocks after filtering.
        /// </summary>
        public ObservableCollection<HomepageStock> FilteredFavoriteStocks
        {
            get => this.filteredFavoriteStocks;
            private set
            {
                this.filteredFavoriteStocks = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current user's CNP identifier.
        /// </summary>
        public string GetUserCNP => this.service.GetUserCNP();

        /// <summary>
        /// Gets or sets a value indicating whether the current user is a guest.
        /// Changing this updates the visibility of profile/guest buttons.
        /// </summary>
        public bool IsGuestUser
        {
            get => this.isGuestUser;
            set
            {
                this.isGuestUser = value;
                // Inline comment: toggle button visibilities based on guest status
                this.GuestButtonVisibility = this.isGuestUser ? Visibility.Visible : Visibility.Collapsed;
                this.ProfileButtonVisibility = this.isGuestUser ? Visibility.Collapsed : Visibility.Visible;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CanModifyFavorites));
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the "Create Profile" button.
        /// </summary>
        public Visibility GuestButtonVisibility
        {
            get => this.guestButtonVisibility;
            set
            {
                this.guestButtonVisibility = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the "Profile" button.
        /// </summary>
        public Visibility ProfileButtonVisibility
        {
            get => this.profileButtonVisibility;
            set
            {
                this.profileButtonVisibility = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current search query for filtering stocks.
        /// </summary>
        public string SearchQuery
        {
            get => this.searchQuery;
            set
            {
                this.searchQuery = value;
                this.OnPropertyChanged();
                // Inline comment: re-apply filter whenever the query changes
                this.ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected sort option.
        /// </summary>
        public string SelectedSortOption
        {
            get => this.selectedSortOption;
            set
            {
                this.selectedSortOption = value;
                this.OnPropertyChanged();
                // Inline comment: re-apply sort whenever the option changes
                this.ApplySort();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is allowed to modify favorites.
        /// </summary>
        public bool CanModifyFavorites => !this.isGuestUser;

        /// <summary>
        /// Loads both all stocks and favorite stocks from the service.
        /// </summary>
        private void LoadStocks()
        {
            // FIXME: '[.. this.service.GetAllStocks()]' is invalid syntax; should construct a new ObservableCollection from the result
            this.FilteredAllStocks = [.. this.service.GetAllStocks()];
            // FIXME: '[.. this.service.GetFavoriteStocks()]' is invalid syntax; should construct a new ObservableCollection from the result
            this.FilteredFavoriteStocks = [.. this.service.GetFavoriteStocks()];
        }

        /// <summary>
        /// Applies the current search filter to the stock lists.
        /// </summary>
        public void ApplyFilter()
        {
            this.service.FilterStocks(this.searchQuery);
            this.LoadStocks();
        }

        /// <summary>
        /// Applies the current sort option to the stock lists.
        /// </summary>
        public void ApplySort()
        {
            this.service.SortStocks(this.selectedSortOption);
            this.LoadStocks();
        }

        /// <summary>
        /// Creates a new user profile and refreshes the view.
        /// </summary>
        public void CreateUserProfile()
        {
            this.service.CreateUserProfile();
            this.IsGuestUser = false;
            this.LoadStocks();
        }

        /// <summary>
        /// Toggles the favorite status of the specified stock.
        /// </summary>
        /// <param name="stock">The <see cref="HomepageStock"/> to toggle; no action if null.</param>
        public void ToggleFavorite(HomepageStock stock)
        {
            if (stock == null)
            {
                return;
            }

            // Inline comment: add or remove from favorites based on current state
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

        /// <summary>
        /// Navigates to a page identified by the given parameter.
        /// </summary>
        /// <param name="parameter">The page name or parameter to navigate to.</param>
        /// <exception cref="ArgumentException">Thrown if the page name is unrecognized.</exception>
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

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed. Auto‑supplied if omitted.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
