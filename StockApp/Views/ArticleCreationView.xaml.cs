namespace StockApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;
    using System;

    public sealed partial class ArticleCreationView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleCreationView"/> class.
        /// </summary>
        public ArticleCreationView(ArticleCreationViewModel viewModel)
        {
            this.ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.DataContext = this.ViewModel;
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the view model for the article creation view.
        /// </summary>
        public ArticleCreationViewModel ViewModel { get; }
    }
}