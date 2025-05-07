namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IActivityRepository
    {
        void AddActivity(string userCNP, string activityName, int amount, string details);

        List<ActivityLog> GetActivityForUser(string userCNP);
    }
}
