using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Services;

namespace StockApp.Repositories.Api
{
    public class ActivityProxyRepo : IActivityRepository
    {
        private readonly ActivityApiService _apiService;

        public ActivityProxyRepo(ActivityApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task<List<ActivityLog>> GetActivityForUserAsync(string userCnp)
        {
            return await _apiService.GetActivityForUser(userCnp);
        }

        public async Task<ActivityLog> AddActivityAsync(string userCnp, string activityName, int amount, string details)
        {
            return await _apiService.AddActivity(userCnp, activityName, amount, details);
        }

        public async Task<List<ActivityLog>> GetAllActivitiesAsync()
        {
            return await _apiService.GetAllActivities();
        }

        public async Task<ActivityLog> GetActivityByIdAsync(int id)
        {
            return await _apiService.GetActivityById(id);
        }

        public async Task<bool> DeleteActivityAsync(int id)
        {
            return await _apiService.DeleteActivity(id);
        }
    }
} 