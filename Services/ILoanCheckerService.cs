namespace StockApp.Services
{
    using System;

    public interface ILoanCheckerService
    {
        event EventHandler LoansUpdated;

        void Start();

        void Stop();
    }
}
