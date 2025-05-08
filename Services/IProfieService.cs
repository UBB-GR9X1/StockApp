﻿namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IProfieService
    {
        string GetDescription();

        string GetImage();

        string GetLoggedInUserCnp();

        string GetUsername();

        List<Stock> GetUserStocks();

        bool IsAdmin();

        bool IsHidden();

        void UpdateIsAdmin(bool isAdmin);

        void UpdateUser(string newUsername, string newImage, string newDescription, bool newHidden);
    }
}