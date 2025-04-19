namespace StockApp.ViewModels
{
    using System;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;

    /// <summary>
    /// Adapter for <see cref="TextBlock"/> to abstract UI element interactions.
    /// </summary>
    public class TextBlockAdapter : ITextBlock
    {
        private readonly TextBlock _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockAdapter"/> class.
        /// </summary>
        /// <param name="inner">The underlying <see cref="TextBlock"/> control to wrap.</param>
        public TextBlockAdapter(TextBlock inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>
        /// Gets or sets the text content of the underlying <see cref="TextBlock"/>.
        /// </summary>
        public string Text
        {
            get => _inner.Text;
            set => _inner.Text = value;
        }

        /// <summary>
        /// Gets or sets the foreground brush of the underlying <see cref="TextBlock"/>.
        /// </summary>
        public object Foreground
        {
            get => _inner.Foreground;
            set
            {
                // Inline: cast and assign the brush to the TextBlock control
                _inner.Foreground = (Brush)value;
            }
        }
    }
}
