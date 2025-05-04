namespace Src.Views
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Models;
    using StockApp.Services;
    using StockApp.Views.Components;

    public sealed partial class UsersView : Page
    {
        private readonly IUserService userService;
        private readonly Func<UserInfoComponent> userComponentFactory;

        public UsersView(IUserService userService, Func<UserInfoComponent> userComponentFactory)
        {
            this.InitializeComponent();
            this.userService = userService;
            this.userComponentFactory = userComponentFactory;
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersContainer.Items.Clear();

            try
            {
                List<User> users = userService.GetUsers();
                foreach (var user in users)
                {
                    var userComponent = userComponentFactory();
                    userComponent.SetUserData(user);
                    UsersContainer.Items.Add(userComponent);
                }
            }
            catch (Exception)
            {
                UsersContainer.Items.Add("There are no users to display.");
            }
        }
    }
}
