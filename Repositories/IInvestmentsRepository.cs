namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IInvestmentsRepository
    {
        List<Investment> GetInvestmentsHistory();

        void AddInvestment(Investment investment);

        void UpdateInvestment(int investmentId, string investorCNP, float amountReturned);
    }
}
