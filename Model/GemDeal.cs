using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Model
{
    class GemDeal
    {
        private string title;
        private int gemAmount;
        private double price;
        private bool isSpecial;

        public GemDeal(string title, int gemAmount, double price, bool isSpecial)
        {
            this.title = title;
            this.gemAmount = gemAmount;
            this.price = price;
            this.isSpecial = isSpecial;
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public int GemAmount
        {
            get { return gemAmount; }
            set { gemAmount = value; }
        }

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public bool IsSpecial
        {
            get { return isSpecial; }
            set { isSpecial = value; }
        }
    }
}
