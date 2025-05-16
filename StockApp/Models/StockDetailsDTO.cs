using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace StockApp.Models
{
    public class StockDetailsDTO
    {
        public Stock StockDetails { get; set; }

        public Page PreviousPage { get; set; }

        public StockDetailsDTO(Stock stockDetails, Page previousPage) 
        { 
            this.StockDetails = stockDetails;
            this.PreviousPage = previousPage;
        }
    }
}
