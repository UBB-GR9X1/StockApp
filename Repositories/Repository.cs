namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using StockApp.Models;

    public class MockRepository
    {
        private static MockRepository instance;

        public List<Stock> Stocks { get; } = [];

        private readonly List<Stock> history = [];

        // Tuple<string cnp, UserStock>
        private readonly List<Tuple<string, UserStock>> userStocks = [];
        private readonly List<User> users = [];

        public List<Transaction> Transactions { get; set; } = [];

        // Tuple<string cnp, string stockName>
        private readonly List<Tuple<string, string>> favorites = [];

        public List<GemDeal> Deals { get; } = [];

        public List<Alert> Alerts { get; set; } = [];

        public static MockRepository Instance => instance ??= new();

        public MockRepository()
        {
            // add 5 users
            this.users.Add(new User("1234567890123", "user1", "user1 description", false, "image1", false));
            this.users.Add(new User("1234567890124", "user2", "user2 description", false, "image2", false));
            this.users.Add(new User("1234567890125", "user3", "user3 description", false, "image3", false));
            this.users.Add(new User("1234567890126", "user4", "user4 description", false, "image4", false));
            this.users.Add(new User("1234567890127", "user5", "user5 description", false, "image5", false));

            this.Stocks.Add(new Stock("stock1", "STK1", "1234567890123", 120));
            this.Stocks.Add(new Stock("stock2", "STK2", "1234567890124", 130));
            this.Stocks.Add(new Stock("stock3", "STK3", "1234567890125", 140));
            this.Stocks.Add(new Stock("stock4", "STK4", "1234567890124", 150));
            this.Stocks.Add(new Stock("stock5", "STK5", "1234567890123", 160));

            this.history.Add(new Stock("stock1", "STK1", "1234567890123", 100));
            this.history.Add(new Stock("stock1", "STK1", "1234567890123", 120));
            this.history.Add(new Stock("stock1", "STK1", "1234567890123", 130));
            this.history.Add(new Stock("stock1", "STK1", "1234567890123", 160));

            this.history.Add(new Stock("stock1", "STK1", "1234567890124", 100));
            this.history.Add(new Stock("stock1", "STK1", "1234567890124", 120));
            this.history.Add(new Stock("stock1", "STK1", "1234567890124", 130));
            this.history.Add(new Stock("stock1", "STK1", "1234567890124", 160));

            // THIS IS BRAINDEAD!!! JUST USE A TUPLE OF STOCK NAME AND QUANTITY !!!  :CCCC
            this.userStocks.Add(new Tuple<string, UserStock>("1234567890123", new UserStock("stock1", "STK1", "1234567890123", 10)));
            this.userStocks.Add(new Tuple<string, UserStock>("1234567890124", new UserStock("stock1", "STK1", "1234567890123", 10)));
            this.userStocks.Add(new Tuple<string, UserStock>("1234567890125", new UserStock("stock1", "STK1", "1234567890123", 10)));
            this.userStocks.Add(new Tuple<string, UserStock>("1234567890126", new UserStock("stock1", "STK1", "1234567890123", 10)));
            this.userStocks.Add(new Tuple<string, UserStock>("1234567890127", new UserStock("stock1", "STK1", "1234567890123", 10)));

            // Transaction has no STOCK NAME??? km
            this.Transactions.Add(new Transaction("stock1", "STK1", "1234567890123", "buy", 10, 120, 1200, DateTime.Now, "1234567890123"));
            this.Transactions.Add(new Transaction("stock1", "STK1", "1234567890124", "buy", 10, 120, 1200, DateTime.Now, "1234567890124"));
            this.Transactions.Add(new Transaction("stock1", "STK1", "1234567890125", "buy", 10, 120, 1200, DateTime.Now, "1234567890125"));
            this.Transactions.Add(new Transaction("stock1", "STK1", "1234567890126", "buy", 10, 120, 1200, DateTime.Now, "1234567890126"));
            this.Transactions.Add(new Transaction("stock1", "STK1", "1234567890127", "buy", 10, 120, 1200, DateTime.Now, "1234567890127"));

            this.favorites.Add(new Tuple<string, string>("1234567890123", "stock1"));
            this.favorites.Add(new Tuple<string, string>("1234567890123", "stock2"));
            this.favorites.Add(new Tuple<string, string>("1234567890123", "stock3"));

            this.Deals.Add(new GemDeal("deal1", 150, 130, false));
            this.Deals.Add(new GemDeal("deal2", 150, 130, false));
            this.Deals.Add(new GemDeal("deal3", 150, 130, false));
            this.Deals.Add(new GemDeal("deal4", 150, 130, false));

            this.Alerts.Add(new Alert { AlertId = 1, StockName = "stock1", Name = "Alert1", UpperBound = 150, LowerBound = 100, ToggleOnOff = true });
            this.Alerts.Add(new Alert { AlertId = 2, StockName = "stock2", Name = "Alert2", UpperBound = 200, LowerBound = 150, ToggleOnOff = false });
            this.Alerts.Add(new Alert { AlertId = 3, StockName = "stock3", Name = "Alert3", UpperBound = 250, LowerBound = 200, ToggleOnOff = true });
            this.Alerts.Add(new Alert { AlertId = 4, StockName = "stock4", Name = "Alert4", UpperBound = 300, LowerBound = 250, ToggleOnOff = false });
            this.Alerts.Add(new Alert { AlertId = 5, StockName = "stock5", Name = "Alert5", UpperBound = 350, LowerBound = 300, ToggleOnOff = true });
        }

        public List<Stock> GetStockHistory(string name) => this.history.FindAll(stock => stock.Name == name);

        public List<UserStock> GetUserStocks(string cnp) =>
            this.userStocks.FindAll(stock => stock.Item1 == cnp).ConvertAll(stock => stock.Item2);

        public void UpdateUserStocks(string cnp, List<UserStock> newStocks)
        {
            this.userStocks.RemoveAll(stock => stock.Item1 == cnp);

            foreach (UserStock stock in newStocks)
            {
                this.userStocks.Add(new Tuple<string, UserStock>(cnp, stock));
            }
        }

        public User GetUserInformation(string cnp) => this.users.Find(user => user.CNP == cnp);

        public void UpdateUserInformation(string cnp, User user)
        {
            this.users.RemoveAll(u => u.CNP == cnp);
            this.users.Add(user);
        }

        public void UpdateUserFavorites(string cnp, List<string> newFavorites)
        {
            this.favorites.RemoveAll(favorite => favorite.Item1 == cnp);

            foreach (string favorite in newFavorites)
            {
                this.favorites.Add(new Tuple<string, string>(cnp, favorite));
            }
        }

        public List<string> GetUserFavorites(string cnp) => favorites
            .FindAll(favorite => favorite.Item1 == cnp)
            .ConvertAll(favorite => favorite.Item2);

        // +getPermanentGemDeals(): List<GemDeals>

        // +fetchCurrentUser(): User // WHO ADDED THIS??
        // TODO: ALERTS

        public void AddAlert(Alert alert)
        {
            this.Alerts.Add(alert);
        }

        public void RemoveAlert(int alertId)
        {
            this.Alerts.RemoveAll(alert => alert.AlertId == alertId);
        }
    }
}
