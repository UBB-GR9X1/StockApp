using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Service
{
    public interface IHomepageService
    {
        ObservableCollection<IHomepageStock> AllStocks { get; }

        ObservableCollection<IHomepageStock> FavoriteStocks { get; }

        ObservableCollection<IHomepageStock> FilteredAllStocks { get; }

        ObservableCollection<IHomepageStock> FilteredFavoriteStocks { get; }

        ObservableCollection<IHomepageStock> GetAllStocks();

        ObservableCollection<IHomepageStock> GetFavoriteStocks();

        void FilterStocks(string query);

        void SortStocks(string sortOption);

        void AddToFavorites(IHomepageStock stock);

        void RemoveFromFavorites(IHomepageStock stock);

        bool IsGuestUser();

        string GetUserCNP();

        void CreateUserProfile();
    }
}
