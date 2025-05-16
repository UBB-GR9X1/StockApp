namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Common.Models;
    using Common.Services;

    public class HistoryViewModel : INotifyPropertyChanged
    {
        private readonly IHistoryService _historyService;
        private List<CreditScoreHistory> _weeklyHistory;
        private List<CreditScoreHistory> _monthlyHistory;
        private List<CreditScoreHistory> _yearlyHistory;
        private string _userCnp;

        public event PropertyChangedEventHandler PropertyChanged;

        public HistoryViewModel(IHistoryService historyService)
        {
            this._historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
        }

        public string UserCnp
        {
            get => this._userCnp;
            set
            {
                if (this._userCnp != value)
                {
                    this._userCnp = value;
                    this.LoadHistory();
                    this.OnPropertyChanged();
                }
            }
        }

        public List<CreditScoreHistory> WeeklyHistory
        {
            get => this._weeklyHistory;
            private set
            {
                if (this._weeklyHistory != value)
                {
                    this._weeklyHistory = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public List<CreditScoreHistory> MonthlyHistory
        {
            get => this._monthlyHistory;
            private set
            {
                if (this._monthlyHistory != value)
                {
                    this._monthlyHistory = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public List<CreditScoreHistory> YearlyHistory
        {
            get => this._yearlyHistory;
            private set
            {
                if (this._yearlyHistory != value)
                {
                    this._yearlyHistory = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private async void LoadHistory()
        {
            if (string.IsNullOrWhiteSpace(this._userCnp))
            {
                return;
            }

            try
            {
                this.WeeklyHistory = await this._historyService.GetHistoryWeeklyAsync(this._userCnp);
                this.MonthlyHistory = await this._historyService.GetHistoryMonthlyAsync(this._userCnp);
                this.YearlyHistory = await this._historyService.GetHistoryYearlyAsync(this._userCnp);
            }
            catch (Exception ex)
            {
                // Handle error appropriately
                System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
