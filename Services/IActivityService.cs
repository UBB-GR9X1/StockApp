﻿namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IActivityService
    {
        Task<List<ActivityLog>> GetActivityForUser(string userCNP);

        Task<ActivityLog> AddActivity(string userCnp, string activityName, int amount, string details);

        Task<List<ActivityLog>> GetAllActivities();

        Task<ActivityLog> GetActivityById(int id);

        Task<bool> DeleteActivity(int id);
    }
}
