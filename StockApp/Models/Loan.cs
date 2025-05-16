namespace StockApp.Models
{
    using System;
    using System.ComponentModel;

    public class Loan : INotifyPropertyChanged
    {
        private int id;
        private string userCnp;
        private decimal loanAmount;
        private DateTime applicationDate;
        private DateTime repaymentDate;
        private decimal interestRate;
        private int numberOfMonths;
        private decimal monthlyPaymentAmount;
        private string status;
        private int monthlyPaymentsCompleted;
        private decimal repaidAmount;
        private decimal penalty;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get => this.id;
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.OnPropertyChanged(nameof(this.Id));
                }
            }
        }

        public string UserCnp
        {
            get => this.userCnp;
            set
            {
                if (this.userCnp != value)
                {
                    this.userCnp = value;
                    this.OnPropertyChanged(nameof(this.UserCnp));
                }
            }
        }

        public decimal LoanAmount
        {
            get => this.loanAmount;
            set
            {
                if (this.loanAmount != value)
                {
                    this.loanAmount = value;
                    this.OnPropertyChanged(nameof(this.LoanAmount));
                }
            }
        }

        public DateTime ApplicationDate
        {
            get => this.applicationDate;
            set
            {
                if (this.applicationDate != value)
                {
                    this.applicationDate = value;
                    this.OnPropertyChanged(nameof(this.ApplicationDate));
                }
            }
        }

        public DateTime RepaymentDate
        {
            get => this.repaymentDate;
            set
            {
                if (this.repaymentDate != value)
                {
                    this.repaymentDate = value;
                    this.OnPropertyChanged(nameof(this.RepaymentDate));
                }
            }
        }

        public decimal InterestRate
        {
            get => this.interestRate;
            set
            {
                if (this.interestRate != value)
                {
                    this.interestRate = value;
                    this.OnPropertyChanged(nameof(this.InterestRate));
                }
            }
        }

        public int NumberOfMonths
        {
            get => this.numberOfMonths;
            set
            {
                if (this.numberOfMonths != value)
                {
                    this.numberOfMonths = value;
                    this.OnPropertyChanged(nameof(this.NumberOfMonths));
                    this.OnPropertyChanged(nameof(this.CanPay));
                }
            }
        }

        public decimal MonthlyPaymentAmount
        {
            get => this.monthlyPaymentAmount;
            set
            {
                if (this.monthlyPaymentAmount != value)
                {
                    this.monthlyPaymentAmount = value;
                    this.OnPropertyChanged(nameof(this.MonthlyPaymentAmount));
                }
            }
        }

        public string Status
        {
            get => this.status;
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.OnPropertyChanged(nameof(this.Status));
                    this.OnPropertyChanged(nameof(this.CanPay));
                }
            }
        }

        public int MonthlyPaymentsCompleted
        {
            get => this.monthlyPaymentsCompleted;
            set
            {
                if (this.monthlyPaymentsCompleted != value)
                {
                    this.monthlyPaymentsCompleted = value;
                    this.OnPropertyChanged(nameof(this.MonthlyPaymentsCompleted));
                    this.OnPropertyChanged(nameof(this.CanPay));

                }
            }
        }

        public decimal RepaidAmount
        {
            get => this.repaidAmount;
            set
            {
                if (this.repaidAmount != value)
                {
                    this.repaidAmount = value;
                    this.OnPropertyChanged(nameof(this.RepaidAmount));
                }
            }
        }

        public decimal Penalty
        {
            get => this.penalty;
            set
            {
                if (this.penalty != value)
                {
                    this.penalty = value;
                    this.OnPropertyChanged(nameof(this.Penalty));
                }
            }
        }

        public bool CanPay => this.MonthlyPaymentsCompleted < this.NumberOfMonths && this.Status == "Approved";

        public Loan() { }

        public Loan(string userCnp, decimal loanAmount, DateTime applicationDate, DateTime repaymentDate, decimal interestRate, int numberOfMonths, decimal monthlyPaymentAmount, int monthlyPaymentsCompleted, decimal repaidAmount, decimal penalty, string status)
        {
            this.UserCnp = userCnp;
            this.LoanAmount = loanAmount;
            this.ApplicationDate = applicationDate;
            this.RepaymentDate = repaymentDate;
            this.InterestRate = interestRate;
            this.NumberOfMonths = numberOfMonths;
            this.MonthlyPaymentAmount = monthlyPaymentAmount;
            this.Status = status;
            this.MonthlyPaymentsCompleted = monthlyPaymentsCompleted;
            this.RepaidAmount = repaidAmount;
            this.Penalty = penalty;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

