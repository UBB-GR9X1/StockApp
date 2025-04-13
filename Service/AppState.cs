using System;

namespace StockApp.Service
{
    public class AppState
    {
        private static readonly Lazy<AppState> _instance = new Lazy<AppState>(() => new AppState());

        public static AppState Instance => _instance.Value;

        public Model.User CurrentUser { get; set; }

        private AppState()
        {
            CurrentUser = new Model.User("1234567890123", "Caramel", "asdf", false, "imagine", false);
        }
    }
}