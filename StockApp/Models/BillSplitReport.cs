namespace Src.Model
{
    using System;

    public class BillSplitReport
    {
        public int Id { get; set; }

        public string ReportedUserCnp { get; set; }

        public string ReportingUserCnp { get; set; }

        public DateTime DateOfTransaction { get; set; }

        public float BillShare { get; set; }

        public BillSplitReport(int id, string reportedCNP, string reporterCNP, DateTime dateTransaction, float billShare)
        {
            this.Id = id;
            this.ReportedUserCnp = reportedCNP;
            this.ReportingUserCnp = reporterCNP;
            this.DateOfTransaction = dateTransaction;
            this.BillShare = billShare;
        }

        public BillSplitReport()
        {
            this.Id = 0;
            this.ReportedUserCnp = string.Empty;
            this.ReportingUserCnp = string.Empty;
            this.DateOfTransaction = DateTime.Now;
            this.BillShare = 0;
        }
    }
}