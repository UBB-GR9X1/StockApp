namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IUserService
    {
        public User GetUserByCnp(string cnp);
        public List<User> GetUsers();
    }
}
