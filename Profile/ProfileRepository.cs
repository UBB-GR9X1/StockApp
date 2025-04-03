using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockApp.Model;

namespace StockApp.Profile
{
    class ProfileRepository
    {
        private string cnp;
        private string userCNP; //=from data base (active user)


        public ProfileRepository(string newcnp)
        {
            this.cnp = newcnp;
        }

        public bool checkForCNP()
        {
            return false; // ...
        }

        public string generateUsername()
        {
            List<string> randomUsernames = new List<string>()
            {
                "macaroane_cu_branza",
                "ecler_cu_fistic",
                "franzela_",
                "username1",
                "snitel_cu_piure",
                "ceai_de_musetel",
                "vita_de_vie",
                "paine_cu_pateu",
                "floare_de_tei",
                "cirese_si_visine",
                "inghetata_roz",
                "tort_de_afine",
                "paste_carbonara",
                "amandina",
                "orez_cu_lapte"
            };
            Random randomNr = new Random();
            int randomIndex = randomNr.Next(14);
            return randomUsernames[randomIndex];
        }

        public bool isActiveUser()
        {

            return true;
            /*
            if(cnp != userCNP)
            {
                return false;
            }
            else
            {
                return true;
            }
            */
        }

        public Model.User CurrentUser()
        {
            if(this.checkForCNP() == false)
            {
                Model.User newUser = new Model.User(cnp, this.generateUsername(), "", false, "", false);
                return newUser;
            }
            else 
            {
                Model.User existingUser = new Model.User(cnp, "", "", false, "", false); //get info from database
                return existingUser;
            }
        }
        public List<string> userStocks()
        {
            //get the user's stocks from the data base - make a string: logo + name + quantity + price
            return new List<string>();   
        }




    }
}
