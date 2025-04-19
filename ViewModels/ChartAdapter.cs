namespace StockApp.ViewModels
{
    using System.Collections.Generic;
    using LiveChartsCore;
    using LiveChartsCore.SkiaSharpView.WinUI;

    /// <summary>
    /// Adapts a <see cref="CartesianChart"/> to the <see cref="IChart"/> interface.
    /// </summary>
    public class ChartAdapter : IChart
    {
        private readonly CartesianChart _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAdapter"/> class,
        /// wrapping the provided <see cref="CartesianChart"/>.
        /// </summary>
        /// <param name="inner">The inner CartesianChart instance to adapt.</param>
        public ChartAdapter(CartesianChart inner)
        {
            // Store the underlying chart for delegation
            this._inner = inner;
        }

        /// <summary>
        /// Gets or sets the series to display on the chart.
        /// </summary>
        public IEnumerable<ISeries> Series
        {
            get => this._inner.Series;
            set => this._inner.Series = value;
        }

        /// <summary>
        /// Updates the chart's layout, forcing a re-render.
        /// </summary>
        public void UpdateLayout()
        {
            // Delegate layout update to the underlying chart
            this._inner.UpdateLayout();
        }
    }
}
