namespace StockApp.ViewModels
{
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;

    public class TextBlockAdapter : ITextBlock
    {
        private readonly TextBlock _inner;

        public TextBlockAdapter(TextBlock inner) => _inner = inner;

        public string Text
        {
            get => _inner.Text;
            set => _inner.Text = value;
        }

        public object Foreground
        {
            get => _inner.Foreground;
            set => _inner.Foreground = (Brush)value;
        }
    }
}
