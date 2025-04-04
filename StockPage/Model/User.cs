using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.StockPage.Model
{
    class User
    {
        private string cnp;
        private string name;
        private int gem_balance;

        public User(string cnp, string name, int gem_balance)
        {
            this.cnp = cnp;
            this.name = name;
            this.gem_balance = gem_balance;
        }

        public string CNP
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
            get { return gem_balance; }
            set { gem_balance = value; }
        }
    }
}
