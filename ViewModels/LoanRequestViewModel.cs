namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Src.Data;
    using Src.Model;
    using StockApp.Repositories;
    using StockApp.Services;

    public class LoanRequestViewModel
    {
        private readonly LoanRequestService loanRequestService;

        public ObservableCollection<LoanRequest> LoanRequests { get; set; }

        public LoanRequestViewModel()
        {
            this.loanRequestService = new LoanRequestService(new LoanRequestRepository(new DatabaseConnection()));
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
