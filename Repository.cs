using StockApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp
{
    class Repository
    {
        private static Repository _instance;
        
        private List<Stock> stocks = new List<Stock>();
        private List<Stock> history = new List<Stock>();
        // Tuple<string cnp, UserStock>
        private List<Tuple<string, UserStock>> userStocks = new List<Tuple<string, UserStock>>();
        private List<User> users = new List<User>();
        private List<Transaction> transactions = new List<Transaction>();
        // Tuple<string cnp, string stockName>
        private List<Tuple<string, string>> favorites = new List<Tuple<string, string>>();
        private List<GemDeal> deals = new List<GemDeal>();
        // no alerts

        public Repository()
        {
            // add 5 users
            users.Add(new User("1234567890123", "user1", "user1 description", false, "image1", false));
            users.Add(new User("1234567890124", "user2", "user2 description", false, "image2", false));
            users.Add(new User("1234567890125", "user3", "user3 description", false, "image3", false));
            users.Add(new User("1234567890126", "user4", "user4 description", false, "image4", false));
            users.Add(new User("1234567890127", "user5", "user5 description", false, "image5", false));

            stocks.Add(new Stock("stock1", "STK1", "1234567890123", 120));
            stocks.Add(new Stock("stock2", "STK2", "1234567890124", 130));
            stocks.Add(new Stock("stock3", "STK3", "1234567890125", 140));
            stocks.Add(new Stock("stock4", "STK4", "1234567890124", 150));
            stocks.Add(new Stock("stock5", "STK5", "1234567890123", 160));

            history.Add(new Stock("stock1", "STK1", "1234567890123", 100));
            history.Add(new Stock("stock1", "STK1", "1234567890123", 120));
            history.Add(new Stock("stock1", "STK1", "1234567890123", 130));
            history.Add(new Stock("stock1", "STK1", "1234567890123", 160));

            history.Add(new Stock("stock1", "STK1", "1234567890124", 100));
            history.Add(new Stock("stock1", "STK1", "1234567890124", 120));
            history.Add(new Stock("stock1", "STK1", "1234567890124", 130));
            history.Add(new Stock("stock1", "STK1", "1234567890124", 160));

            // THIS IS BRAINDEAD!!! JUST USE A TUPLE OF STOCK NAME AND QUANTITY !!!  :CCCC
            userStocks.Add(new Tuple<string, UserStock>("1234567890123", new UserStock("stock1", "STK1", "1234567890123", 10)));
            userStocks.Add(new Tuple<string, UserStock>("1234567890124", new UserStock("stock1", "STK1", "1234567890123", 10)));
            userStocks.Add(new Tuple<string, UserStock>("1234567890125", new UserStock("stock1", "STK1", "1234567890123", 10)));
            userStocks.Add(new Tuple<string, UserStock>("1234567890126", new UserStock("stock1", "STK1", "1234567890123", 10)));
            userStocks.Add(new Tuple<string, UserStock>("1234567890127", new UserStock("stock1", "STK1", "1234567890123", 10)));

            // Transaction has no STOCK NAME??? km
            transactions.Add(new Transaction("stock1", "STK1", "1234567890123", "buy", 10, 120, 1200, DateTime.Now, "1234567890123"));
            transactions.Add(new Transaction("stock1", "STK1", "1234567890124", "buy", 10, 120, 1200, DateTime.Now, "1234567890124"));
            transactions.Add(new Transaction("stock1", "STK1", "1234567890125", "buy", 10, 120, 1200, DateTime.Now, "1234567890125"));
            transactions.Add(new Transaction("stock1", "STK1", "1234567890126", "buy", 10, 120, 1200, DateTime.Now, "1234567890126"));
            transactions.Add(new Transaction("stock1", "STK1", "1234567890127", "buy", 10, 120, 1200, DateTime.Now, "1234567890127"));

            favorites.Add(new Tuple<string, string>("1234567890123", "stock1"));
            favorites.Add(new Tuple<string, string>("1234567890123", "stock2"));
            favorites.Add(new Tuple<string, string>("1234567890123", "stock3"));

            deals.Add(new GemDeal("deal1", 150, 130, false));
            deals.Add(new GemDeal("deal2", 150, 130, false));
            deals.Add(new GemDeal("deal3", 150, 130, false));
            deals.Add(new GemDeal("deal4", 150, 130, false));
        }

        public static Repository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Repository();
                }
                return _instance;
            }
        }

        public List<Stock> getStockList()
        {
            return stocks;
        }

        public List<Stock> getStockHistory(string name)
        {
            return history.FindAll(stock => stock.Name == name);
        }
        public List<UserStock> getUserStocks(string cnp)
        {
            return userStocks.FindAll(stock => stock.Item1 == cnp).ConvertAll(stock => stock.Item2);
        }

        public void updateUserStocks(string cnp, List<UserStock> newStocks)
        {
            userStocks.RemoveAll(stock => stock.Item1 == cnp);
            foreach (UserStock stock in newStocks)
            {
                userStocks.Add(new Tuple<string, UserStock>(cnp, stock));
            }
        }

        public User getUserInformation(string cnp)
        {
            return users.Find(user => user.CNP == cnp);
        }
        public void updateUserInformation(string cnp, User user)
        {
            users.RemoveAll(u => u.CNP == cnp);
            users.Add(user);
        }

        public void updateUserFavorites(string cnp, List<string> newFavorites)
        {
            favorites.RemoveAll(favorite => favorite.Item1 == cnp);
            foreach (string favorite in newFavorites)
            {
                favorites.Add(new Tuple<string, string>(cnp, favorite));
            }
        }

        public List<string> getUserFavorites(string cnp)
        {
            return favorites.FindAll(favorite => favorite.Item1 == cnp).ConvertAll(favorite => favorite.Item2);
        }

        public List<Transaction> getAllTransactions()
        {
            return transactions;
        }

        public void updateAllTransactions(List<Transaction> newList)
        {
            transactions = newList;
        }

        // +getPermanentGemDeals(): List<GemDeals>
        public List<GemDeal> getPermanentGemDeals()
        {
            return deals;
        }

        // +fetchCurrentUser(): User // WHO ADDED THIS??
        // TODO: ALERTS
    }

}
