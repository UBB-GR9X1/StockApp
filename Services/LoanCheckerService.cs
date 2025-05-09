namespace StockApp.Services
{
    using System;
    using Microsoft.UI.Xaml;

    public class LoanCheckerService : ILoanCheckerService
    {
        private readonly ILoanService loanServices;
        private readonly DispatcherTimer timer;

        public event EventHandler LoansUpdated;

        public LoanCheckerService(ILoanService loanServices)
        {
            this.loanServices = loanServices;
            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // set to 1 second for testing purposes, should be higher since this checks for monthly payments
            };
            this.timer.Tick += this.Timer_Tick;
        }

        private async void Timer_Tick(object sender, object e)
        {
            await this.loanServices.CheckLoansAsync();
            this.LoansUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }
    }
}
