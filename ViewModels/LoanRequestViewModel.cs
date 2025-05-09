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
        public bool IsLoading { get; private set; }
        public string ErrorMessage { get; private set; }

        public LoanRequestViewModel(ILoanRequestService loanService)
        {
            this.loanRequestService = loanService;
            this.LoanRequests = new ObservableCollection<LoanRequest>();
        }

        public async Task LoadLoanRequestsAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            LoanRequests.Clear();

            try
            {
                var requests = await _loanRequestService.GetLoanRequests();
                foreach (var request in requests)
                {
                    LoanRequests.Add(request);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load loan requests: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<string> GetSuggestionAsync(LoanRequest loanRequest)
        {
            if (loanRequest == null)
            {
                return "Invalid loan request.";
            }

            try
            {
                return await Task.Run(() => _loanRequestService.GiveSuggestion(loanRequest));
            }
            catch (Exception ex)
            {
                return $"Failed to get suggestion: {ex.Message}";
            }
        }
    }
}
