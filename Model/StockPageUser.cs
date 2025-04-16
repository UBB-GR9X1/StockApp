namespace StockApp.Model
{
    public class StockPageUser
    {
        private string cnp;
        private string name;
        private int gemBalance;

        public StockPageUser(string cnp, string name, int gemBalance)
        {
            this.cnp = cnp;
            this.name = name;
            this.gemBalance = gemBalance;
        }

        public string Cnp
        {
            get { return cnp; }
            set { cnp = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int GemBalance
        {
            get { return gemBalance; }
            set { gemBalance = value; }
        }
    }
}
