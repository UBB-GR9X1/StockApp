using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockApp.Model;
using Windows.System;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using StockApp.Database;

namespace StockApp.Profile
{
    class ProfileRepository
    {
        private SqlConnection dbConnection = DatabaseHelper.Instance.GetConnection();
        private string cnp; //the user we are currently working with (idk what to do with it so i set it to "userCNP")
        private string userCNP; //=from data base (active user)



public ProfileRepository()
        {
            string getCNPquery = "SELECT CNP FROM [HARDCODED_CNPS] WHERE CNP = '1234567890124'";
            using (var checkCommand = new SqlCommand(getCNPquery, dbConnection))
            {
                this.userCNP = checkCommand.ExecuteScalar().ToString();
                
            }
            this.cnp = this.userCNP; //
        }

        public bool checkForCNP() //if it is in db
        {
            string thecnp = "1234567890124"; //should be this.cnp
            string getCNPquery = "SELECT CNP FROM [USER] WHERE CNP = @CNP";
            using (var checkCommand = new SqlCommand(getCNPquery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", thecnp);
                if (checkCommand.ExecuteScalar() == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
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

            string getCNPquery = "SELECT CNP FROM [USER] WHERE CNP = @CNP";
            using (var checkCommand = new SqlCommand(getCNPquery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
                if (checkCommand.ExecuteScalar() == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }

        public Model.User CurrentUser()
        {
            if(this.checkForCNP() == false)
            {
                Model.User newUser = new Model.User(cnp, this.generateUsername(), "", false, "", false);
                return newUser;
            }
            else 
            { //put this.cnp not user
                string myUsername;
                string getUsernamequery = "SELECT NAME FROM [USER] WHERE CNP = @CNP";
                using (var checkCommand = new SqlCommand(getUsernamequery, dbConnection))
                {
                    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
                    myUsername = checkCommand.ExecuteScalar().ToString();
                }

                string myImage;
                string getImagequery = "SELECT PROFILE_PICTURE FROM [USER] WHERE CNP = @CNP";
                using (var checkCommand = new SqlCommand(getImagequery, dbConnection))
                {
                    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
                    myImage = checkCommand.ExecuteScalar().ToString();
                }

                string myDescription;
                string getDescriptionquery = "SELECT DESCRIPTION FROM [USER] WHERE CNP = @CNP";
                using (var checkCommand = new SqlCommand(getDescriptionquery, dbConnection))
                {
                    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
                    myDescription = checkCommand.ExecuteScalar().ToString();
                }

                int hiddenornot;
                bool isH;
                string getHiddenquery = "SELECT IS_HIDDEN FROM [USER] WHERE CNP = @CNP";
                using (var checkCommand = new SqlCommand(getHiddenquery, dbConnection))
                {
                    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
                    hiddenornot = Convert.ToInt32(checkCommand.ExecuteScalar());
                }
                if(hiddenornot==1)isH = true;
                else isH = false;

                int adminornot;
                bool isA;
                string getAdminquery = "SELECT IS_ADMIN FROM [USER] WHERE CNP = @CNP";
                using (var checkCommand = new SqlCommand(getAdminquery, dbConnection))
                {
                    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
                    adminornot = Convert.ToInt32(checkCommand.ExecuteScalar());
                }
                if (adminornot == 1) isA = true;
                else isA = false;


                //Model.User existingUser = new Model.User(cnp, "", "", false, "", false); //get info from database
                Model.User existingUser = new Model.User(this.cnp, myUsername, myDescription, isA, myImage, isH);
                return existingUser;
            }
        }

        public void updateRepoIsAdmin(bool newisA)
        {
            int myisA;
            if (newisA == true) myisA = 1;
            else myisA = 0;
                string updateQuery = "UPDATE [USER] SET IS_ADMIN = @IsAdmin WHERE CNP = @CNP";
            using (var updateCommand = new SqlCommand(updateQuery, dbConnection))
            {
                updateCommand.Parameters.AddWithValue("@IsAdmin", myisA.ToString());
                updateCommand.Parameters.AddWithValue("@CNP", cnp);
                updateCommand.ExecuteNonQuery();
            }
        }

        public void updateMyUser(string newUsername, string newImage, string newDescription, bool newHidden)
        {//update stuff - see serv

            string newHiddenint;
            if(newHidden == true) newHiddenint = "1";
            else newHiddenint = "0";

            string updateUserquery = @"UPDATE [USER] SET NAME = @NewName, PROFILE_PICTURE = @NewProfilePicture, DESCRIPTION = @NewDescription, IS_HIDDEN = @NewIsHidden WHERE CNP = @CNP";
            using (var updateCommand = new SqlCommand(updateUserquery, dbConnection))
            {
                updateCommand.Parameters.AddWithValue("@NewName", newUsername);
                updateCommand.Parameters.AddWithValue("@NewProfilePicture",newImage);
                updateCommand.Parameters.AddWithValue("@NewDescription",newDescription);
                updateCommand.Parameters.AddWithValue("@NewIsHidden",newHiddenint);
                updateCommand.Parameters.AddWithValue("@CNP",this.cnp);
                updateCommand.ExecuteNonQuery();
            }



        }

        public List<string> userStocks()
        {
            List<string> stocks = new List<string>();

            string query = @"
        SELECT S.STOCK_SYMBOL, US.STOCK_NAME, US.QUANTITY, SV.PRICE
        FROM USER_STOCK US
        JOIN STOCK_VALUE SV ON US.STOCK_NAME = SV.STOCK_NAME
        JOIN STOCK S ON US.STOCK_NAME = S.STOCK_NAME
        WHERE US.USER_CNP = @CNP";

            using (var command = new SqlCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@CNP", this.cnp);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string symbol = reader["STOCK_SYMBOL"].ToString();
                        string stockName = reader["STOCK_NAME"].ToString();
                        int quantity = Convert.ToInt32(reader["QUANTITY"]);
                        int price = Convert.ToInt32(reader["PRICE"]);

                        string stockString = $"{symbol} | {stockName} | Quantity: {quantity} | Price: {price}";
                        stocks.Add(stockString);
                    }
                }
            }

            return stocks;
        }


        //public List<string> userStocks()
        //{



        //    //string mySymbol;
        //    //string getSymquery = "SELECT STOCK_SYMBOL FROM [USER_STOCK] WHERE USER_CNP = @CNP";
        //    //using (var checkCommand = new SqlCommand(getSymquery, dbConnection))
        //    //{
        //    //    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
        //    //    if(checkCommand.ExecuteScalar() == null) mySymbol = string.Empty;
        //    //    else mySymbol = checkCommand.ExecuteScalar().ToString();
        //    //}

        //    //string myStockName;
        //    //string getStockNamequery = "SELECT NAME FROM [USER_STOCK] WHERE USER_CNP = @CNP";
        //    //using (var checkCommand = new SqlCommand(getStockNamequery, dbConnection))
        //    //{
        //    //    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
        //    //    if(checkCommand.ExecuteScalar() == null) myStockName = string.Empty;    
        //    //    else myStockName = checkCommand.ExecuteScalar().ToString();
        //    //}

        //    //string myQuantity;
        //    //string getStockQuantituquery = "SELECT NAME FROM [USER_STOCK] WHERE USER_CNP = @CNP";
        //    //using (var checkCommand = new SqlCommand(getStockQuantituquery, dbConnection))
        //    //{
        //    //    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
        //    //    if (checkCommand.ExecuteScalar() == null) { myQuantity = string.Empty; }    
        //    //    else myQuantity = checkCommand.ExecuteScalar().ToString();
        //    //}

        //    //stock LIST




        //    //get the user's stocks from the data base - make a string: logo + name + quantity + price
        //    //_user = new User("1234567890", "Caramel", "asdf", false, "https://static.wikia.nocookie.net/hellokitty/images/3/32/Sanrio_Characters_Keroppi_Image007.png/revision/latest/thumbnail/width/360/height/360?cb=20170405011801", false);
        //    //return new List<string> { "Stock A", "Stock B", "Stock C", "Stock D" };
        //   // return new List<string>();   
        //}




    }
}
