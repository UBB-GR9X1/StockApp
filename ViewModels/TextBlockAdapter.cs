namespace StockApp.ViewModels
{
    using System;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;

    /// <summary>
    /// Adapter for <see cref="TextBlock"/> to abstract UI element interactions.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TextBlockAdapter"/> class.
    /// </remarks>
    /// <param name="inner">The underlying <see cref="TextBlock"/> control to wrap.</param>
    public class TextBlockAdapter(TextBlock inner) : ITextBlock
    {
        private readonly TextBlock inner = inner ?? throw new ArgumentNullException(nameof(inner));

        /// <summary>
        /// Gets or sets the text content of the underlying <see cref="TextBlock"/>.
        /// </summary>
        public string Text
        {
            get => this.inner.Text;
            set => this.inner.Text = value;
        }

        /// <summary>
        /// Gets or sets the foreground brush of the underlying <see cref="TextBlock"/>.
        /// </summary>
        public object Foreground
        {
            get => this.inner.Foreground;
            set
            {
                // Inline: cast and assign the brush to the TextBlock control
                this.inner.Foreground = (Brush)value;
            }
        }
    }
}
