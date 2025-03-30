//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using StocksHomepage.Model;
//using StocksHomepage.Service;
//using System.Linq;

//namespace StocksHomepage.ViewModel
//{
//    public class HomepageViewModel
//    {
//        public ObservableCollection<Stock> FavoriteFilteredStocks { get; set; }
//        public ObservableCollection<Stock> AllFilteredStocks { get; set; }

//        private HomepageService _service;

//        public HomepageViewModel()
//        {
//            _service = new HomepageService();
//            InitializeStocks();
//        }

//        // Initialize stocks from the service (in-memory)
//        private void InitializeStocks()
//        {
//            var allStocks = _service.GetAllStocks();
//            var favoriteStocks = _service.GetFavoriteStocks();

//            // Initialize ObservableCollections with the fetched data
//            FavoriteFilteredStocks = new ObservableCollection<Stock>(favoriteStocks);
//            AllFilteredStocks = new ObservableCollection<Stock>(allStocks.Where(stock => !favoriteStocks.Contains(stock)));
//        }

//        public void FilterStocks(string query, string sortOption)
//        {
//            // Get all stocks and favorites from the service
//            var allStocks = _service.GetAllStocks();
//            var favoriteStocks = _service.GetFavoriteStocks();

//            // Filter the stocks based on the query (using LINQ)
//            var filteredAll = allStocks.Where(stock => stock.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
//            var filteredFavorites = favoriteStocks.Where(stock => stock.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

//            // Apply sorting
//            ApplySorting(filteredAll, filteredFavorites, sortOption);

//            // Update ObservableCollections
//            FavoriteFilteredStocks.Clear();
//            AllFilteredStocks.Clear();

//            foreach (var stock in filteredAll)
//            {
//                AllFilteredStocks.Add(stock);
//            }

//            foreach (var stock in filteredFavorites)
//            {
//                FavoriteFilteredStocks.Add(stock);
//            }
//        }

//        private void ApplySorting(List<Stock> allFiltered, List<Stock> favoriteFiltered, string sortOption)
//        {
//            switch (sortOption)
//            {
//                case "Sort by Name":
//                    allFiltered.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
//                    favoriteFiltered.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
//                    break;
//                case "Sort by Price":
//                    allFiltered.Sort((x, y) => decimal.Compare(decimal.Parse(x.Price.Trim('$')), decimal.Parse(y.Price.Trim('$'))));
//                    favoriteFiltered.Sort((x, y) => decimal.Compare(decimal.Parse(x.Price.Trim('$')), decimal.Parse(y.Price.Trim('$'))));
//                    break;
//                case "Sort by Change":
//                    allFiltered.Sort((x, y) => decimal.Compare(decimal.Parse(x.Change.Trim('%')), decimal.Parse(y.Change.Trim('%'))));
//                    favoriteFiltered.Sort((x, y) => decimal.Compare(decimal.Parse(x.Change.Trim('%')), decimal.Parse(y.Change.Trim('%'))));
//                    break;
//            }

//            // Clear the existing ObservableCollection
//            FavoriteFilteredStocks.Clear();
//            AllFilteredStocks.Clear();

//            // Add the sorted items back to the ObservableCollection
//            foreach (var stock in allFiltered)
//            {
//                AllFilteredStocks.Add(stock);
//            }

//            foreach (var stock in favoriteFiltered)
//            {
//                FavoriteFilteredStocks.Add(stock);
//            }
//        }

//        public void AddToFavorites(Stock stock)
//        {
//            // Check if the stock is already in favorites
//            if (!FavoriteFilteredStocks.Contains(stock))
//            {
//                // Add to favorites (in-memory service update)
//                _service.AddToFavorites(stock);

//                // Remove from AllFilteredStocks if it's there
//                if (AllFilteredStocks.Contains(stock))
//                {
//                    AllFilteredStocks.Remove(stock);
//                }

//                // Add to FavoriteFilteredStocks
//                FavoriteFilteredStocks.Add(stock);
//            }
//            else
//            {
//                // If it's already in favorites, do nothing
//                // Optionally, you could add a check here to alert the user or provide feedback
//            }
//        }

//        public void RemoveFromFavorites(Stock stock)
//        {
//            // Check if the stock is in favorites
//            if (FavoriteFilteredStocks.Contains(stock))
//            {
//                // Remove from favorites (in-memory service update)
//                _service.RemoveFromFavorites(stock);

//                // Remove from FavoriteFilteredStocks
//                FavoriteFilteredStocks.Remove(stock);

//                // Add back to AllFilteredStocks (only if it should be in there)
//                if (!AllFilteredStocks.Contains(stock))
//                {
//                    AllFilteredStocks.Add(stock);
//                }
//            }
//            else
//            {
//                // If it's not in favorites, do nothing
//                // You can optionally add a check here to notify the user
//            }
//        }


//        // Refresh stocks after modification
//        private void RefreshStocks()
//        {
//            var allStocks = _service.GetAllStocks();
//            var favoriteStocks = _service.GetFavoriteStocks();

//            // Clear current ObservableCollections
//            FavoriteFilteredStocks.Clear();
//            AllFilteredStocks.Clear();

//            // Add favorite stocks to FavoriteFilteredStocks
//            foreach (var stock in favoriteStocks)
//            {
//                FavoriteFilteredStocks.Add(stock);
//            }

//            // Add all stocks (excluding favorites) to AllFilteredStocks
//            foreach (var stock in allStocks.Where(stock => !favoriteStocks.Contains(stock)).ToList())
//            {
//                AllFilteredStocks.Add(stock);
//            }
//        }
//    }
//}
