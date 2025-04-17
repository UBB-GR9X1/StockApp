using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Models
{
    public interface IUser
    {
        string Cnp { get; set; }

        string Username { get; set; }

        string Description { get; set; }

        bool IsModerator { get; set; }

        string Image { get; set; }

        bool IsHidden { get; set; }
    }
}
