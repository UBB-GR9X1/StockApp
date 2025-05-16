using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace StockApp.Repositories
{
    public interface IInvestmentsRepository
    {
        Task<List<Investment>> GetInvestmentsHistory();

        Task AddInvestment(Investment investment);

        Task UpdateInvestment(int investmentId, string investorCNP, float amountReturned);
    }
}
