using System.Collections.ObjectModel;
using StockApp.Models;

namespace StockApp.Services
{
    public interface IHomepageService
    {
        ObservableCollection<HomepageStock> AllStocks { get; }

        ObservableCollection<HomepageStock> FavoriteStocks { get; }

        ObservableCollection<HomepageStock> FilteredAllStocks { get; }

        ObservableCollection<HomepageStock> FilteredFavoriteStocks { get; }

        ObservableCollection<HomepageStock> GetAllStocks();

        ObservableCollection<HomepageStock> GetFavoriteStocks();

        void FilterStocks(string query);

        void SortStocks(string sortOption);

        void AddToFavorites(HomepageStock stock);

        void RemoveFromFavorites(HomepageStock stock);

        bool IsGuestUser();

        string GetUserCNP();

        void CreateUserProfile();
    }
}
