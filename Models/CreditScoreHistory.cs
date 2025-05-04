namespace Src.Model
{
    using System;

    public class CreditScoreHistory
    {
        public int Id { get; set; }
        public string UserCnp { get; set; }
        public DateOnly Date { get; set; }
        public int Score { get; set; }

        public CreditScoreHistory(int id, string userCnp, DateOnly date, int creditScore)
        {
            this.Id = id;
            this.UserCnp = userCnp;
            this.Date = date;
            this.Score = creditScore;
        }

        public CreditScoreHistory()
        {
            this.Id = 0;
            this.UserCnp = string.Empty;
            this.Date = new DateOnly();
            this.Score = 0;
        }
    }
}
