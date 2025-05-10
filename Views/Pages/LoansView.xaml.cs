namespace StockApp.Views.Pages
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class LoansView : Page
    {
        public LoansView(LoansViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            //this.Loaded += (s, e) =>
            //{
            //    if (viewModel.LoadLoansCommand.CanExecute(null))
            //    {
            //        viewModel.LoadLoansCommand.Execute(null);
            //    }
            //};
        }
    }
}
