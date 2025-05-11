namespace StockApp.Views.Pages
{
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    public sealed partial class LoansView : Page
    {
        private readonly LoansViewModel viewModel;

        public LoansView(LoansViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Loaded += (s, e) =>
            {
                if (viewModel.LoadLoansCommand.CanExecute(null))
                {
                    viewModel.LoadLoansCommand.Execute(null);
                }
            };
        }

        private void LoanComponent_LoanUpdated(object sender, System.EventArgs e)
        {
            this.viewModel.OnLoansUpdatedCommand.Execute(null);
        }
    }
}
