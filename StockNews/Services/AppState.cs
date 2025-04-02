using StockNewsPage.Models;
using System;

namespace StockNewsPage.Services
{
    public class AppState
    {
        private static readonly Lazy<AppState> _instance = new Lazy<AppState>(() => new AppState());

        public static AppState Instance => _instance.Value;

        public StockApp.Model.User CurrentUser { get; set; }

        private AppState()
        {
            CurrentUser = new StockApp.Model.User("1234567890", "Caramel", "asdf", false, "imagine", false);
        }
    }
}