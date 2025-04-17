namespace StockApp.Service
{
    using System;
    using StockApp.Models;

    public class AppState
    {
        private static readonly Lazy<AppState> InstanceValue = new (() => new AppState());

        public static AppState Instance => InstanceValue.Value;

        public Model.User CurrentUser 
        { 
            get => _currentUser;
            set => _currentUser = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Model.User _currentUser;

        private AppState()
        {
            this.CurrentUser = new ("1234567890123", "Caramel", "asdf", false, "imagine", false);
        }
    }
}