using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionLog
{
    public class TransactionFilterCriteria
    {
        public string? StockName { get; set; }
        public string? Type { get; set; }
        public int? MinTotalValue { get; set; }
        public int? MaxTotalValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public void Validate()
        {
            if (!string.IsNullOrEmpty(Type) && !(Type.Equals("BUY") || Type.Equals("SELL")))
                throw new Exception("The type must be \"BUY\" or \"SELL\"!");
            if ((MinTotalValue.HasValue && MaxTotalValue.HasValue) && (MinTotalValue > MaxTotalValue))
                throw new Exception("The min total value cannot be greater than the max total value!");
            if ((StartDate.HasValue && EndDate.HasValue) && (StartDate > EndDate))
                throw new Exception("The start date cannot be chronologically after the end date!");
        }
    }
}
