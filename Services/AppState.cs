namespace StockApp.Services
{
    using System;
    using StockApp.Models;

    public class AppState : IAppState
    {
        private static readonly Lazy<AppState> InstanceValue = new(() => new AppState());

        /// <summary>
        /// Singleton instance of the AppState class.
        /// </summary>
        public static AppState Instance => InstanceValue.Value;

        /// <summary>
        /// Gets or sets the current user of the application.
        /// </summary>
        public User CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value ?? throw new ArgumentNullException(nameof(value));
        }

        private User _currentUser;

        private AppState()
        {
            this.CurrentUser = new("1234567890123", "Caramel", "asdf", false, "imagine", false, 123);
        }
    }
}