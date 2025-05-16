using Common.Models;
using Microsoft.UI.Xaml.Controls;

namespace StockApp.DTO
{
    public class StockDetailsDTO
    {
        public Stock StockDetails { get; set; }

        public Page PreviousPage { get; set; }

        public StockDetailsDTO(Stock stockDetails, Page previousPage)
        {
            StockDetails = stockDetails;
            PreviousPage = previousPage;
        }
    }
}
