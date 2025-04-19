namespace StockApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class ArticleCreationView : Page
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleCreationView"/> class.
        /// </summary>
        public ArticleCreationView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets the view model for the article creation view.
        /// </summary>
        public ArticleCreationViewModel ViewModel { get; } = new ();

        /// <summary>
        /// Handles the Loaded event of the ArticleCreationView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.ViewModel.Initialize();
        }
    }
}