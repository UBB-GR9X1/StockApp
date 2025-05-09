namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Services;

    public class LoanRequestViewModel
    {
        private readonly ILoanRequestService loanRequestService;

        public ObservableCollection<LoanRequest> LoanRequests { get; set; }

        public LoanRequestViewModel(ILoanRequestService loanService)
        {
            this.loanRequestService = loanService;
            this.LoanRequests = new ObservableCollection<LoanRequest>();
        }

        public async Task LoadLoanRequests()
        {
            try
            {
                var requests = this.loanRequestService.GetLoanRequests();
                foreach (var request in requests)
                {
                    this.LoanRequests.Add(request);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
        }
    }
}
