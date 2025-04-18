namespace StockApp.ViewModels
{
    using System.Collections.Generic;
    using LiveChartsCore;

    public interface IChart
    {
        IEnumerable<ISeries> Series { get; set; }

        void UpdateLayout();
    }
}
