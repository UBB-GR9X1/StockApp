using System.Collections.Generic;
using BankApi.Models;

namespace BankApi.Repositories
{
    public interface IInvestmentsRepository
    {
        List<Investment> GetInvestmentsHistory();
        void AddInvestment(Investment investment);
        void UpdateInvestment(int investmentId, string investorCNP, float amountReturned);
    }
} 