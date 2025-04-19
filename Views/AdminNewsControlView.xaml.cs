namespace StockApp.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class AdminNewsControlView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminNewsControlView"/> class.
        /// </summary>
        public AdminNewsControlView()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets the view model for the AdminNewsControlView.
        /// </summary>
        public AdminNewsViewModel ViewModel { get; } = new ();

        /// <summary>
        /// Handles the Loaded event of the AdminNewsControlView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Initialize();
        }
    }
}
