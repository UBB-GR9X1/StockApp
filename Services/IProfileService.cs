﻿namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IProfileService
    {
        string GetImage();

        string GetLoggedInUserCnp();

        string GetUsername();

        string GetDescription();

        bool IsHidden();

        bool IsAdmin();

        List<Stock> GetUserStocks();

        void UpdateIsAdmin(bool newIsAdmin);

        void UpdateUser(string newUsername, string newImage, string newDescription, bool newHidden);
    }
}