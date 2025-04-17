using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Models
{
    public interface IBaseStock
    {
        string Name { get; }

        string Symbol { get; }

        string AuthorCnp { get; }
    }
}
