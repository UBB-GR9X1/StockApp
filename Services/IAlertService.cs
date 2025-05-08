﻿namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IAlertService
    {
        Task<Alert> CreateAlertAsync(string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff);
        Task<Alert?> GetAlertByIdAsync(int alertId);
        Task<List<Alert>> GetAllAlertsAsync();
        Task<List<Alert>> GetAllAlertsOnAsync();
        Task RemoveAlertAsync(int alertId);
        Task UpdateAlertAsync(Alert alert);
        Task UpdateAlertAsync(int alertId, string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff);
    }
}