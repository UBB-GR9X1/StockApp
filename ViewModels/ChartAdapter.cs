namespace StockApp.ViewModels
{
    using System.Collections.Generic;
    using LiveChartsCore;
    using LiveChartsCore.SkiaSharpView.WinUI;

    public class ChartAdapter : IChart
    {
        private readonly CartesianChart _inner;

        public ChartAdapter(CartesianChart inner) => _inner = inner;

        public IEnumerable<ISeries> Series
        {
            get => _inner.Series;
            set => _inner.Series = value;
        }

        public void UpdateLayout() => _inner.UpdateLayout();
    }
}
