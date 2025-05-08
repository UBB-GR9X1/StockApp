﻿namespace StockApp.Services
{
    using System;
    using Microsoft.UI.Xaml.Controls;

    public interface INavigationService
    {
        static abstract NavigationService Instance { get; }

        bool CanGoBack { get; }

        static abstract void Initialize(INavigationFrame frame);

        void GoBack();

        bool Navigate(Type pageType, object? parameter = null);

        void NavigateToArticleDetail(string articleId);
    }
}