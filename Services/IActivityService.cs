namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IActivityService
    {
        List<ActivityLog> GetActivityForUser(string userCNP);
    }
}
