namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IUserService
    {
        User GetUserByCnp(string cnp);

        List<User> GetUsers();
    }
}
