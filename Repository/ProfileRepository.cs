using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using StockApp.Database;

namespace StockApp.Repository
{
    public class ProfileRepository
    {
        private SqlConnection dbConnection = DatabaseHelper.Instance.GetConnection();
        private string cnp; // the user we are currently working with (idk what to do with it so i set it to "userCNP")
        private string userCNP; // from data base (active user)
        ///  I HAVE NO IDEA WHAT UR DOING!!!
        private string loggedInUserCNP;

        public ProfileRepository(string author_cnp)
        {
            string getCNPquery = "SELECT CNP FROM [HARDCODED_CNPS]";
            using (var checkCommand = new SqlCommand(getCNPquery, dbConnection))
            {
                loggedInUserCNP = checkCommand.ExecuteScalar().ToString();
            }

            cnp = author_cnp; // this is the cnp of the user we are working with (the one we want to see the profile of)
            userCNP = cnp;
            // SOMETINES UR USING ONE, SOMETIMES THE OTHER...
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

        public Model.User CurrentUser()
        {
            string myUsername;
            string getUsernamequery = "SELECT NAME FROM [USER] WHERE CNP = @CNP";
            using (var checkCommand = new SqlCommand(getUsernamequery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", userCNP);
                myUsername = checkCommand.ExecuteScalar().ToString();
            }

            string myImage;
            string getImagequery = "SELECT PROFILE_PICTURE FROM [USER] WHERE CNP = @CNP";
            using (var checkCommand = new SqlCommand(getImagequery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", userCNP);
                myImage = checkCommand.ExecuteScalar().ToString();
            }

            string myDescription;
            string getDescriptionquery = "SELECT DESCRIPTION FROM [USER] WHERE CNP = @CNP";
            using (var checkCommand = new SqlCommand(getDescriptionquery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", userCNP);
                myDescription = checkCommand.ExecuteScalar().ToString();
            }

            int hiddenornot;
            bool isH;
            string getHiddenquery = "SELECT IS_HIDDEN FROM [USER] WHERE CNP = @CNP";
            using (var checkCommand = new SqlCommand(getHiddenquery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", userCNP);
                hiddenornot = Convert.ToInt32(checkCommand.ExecuteScalar());
            }
            if (hiddenornot == 1) isH = true;
            else isH = false;

            int adminornot;
            bool isA;
            string getAdminquery = "SELECT IS_ADMIN FROM [USER] WHERE CNP = @CNP";
            using (var checkCommand = new SqlCommand(getAdminquery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", userCNP);
                adminornot = Convert.ToInt32(checkCommand.ExecuteScalar());
            }
            if (adminornot == 1) isA = true;
            else isA = false;

            //Model.User existingUser = new Model.User(cnp, "", "", false, "", false); //get info from database
            Model.User existingUser = new Model.User(cnp, myUsername, myDescription, isA, myImage, isH);
            return existingUser;
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
            if (newHidden == true) newHiddenint = "1";
            else newHiddenint = "0";

            string updateUserquery = @"UPDATE [USER] SET NAME = @NewName, PROFILE_PICTURE = @NewProfilePicture, DESCRIPTION = @NewDescription, IS_HIDDEN = @NewIsHidden WHERE CNP = @CNP";
            using (var updateCommand = new SqlCommand(updateUserquery, dbConnection))
            {
                updateCommand.Parameters.AddWithValue("@NewName", newUsername);
                updateCommand.Parameters.AddWithValue("@NewProfilePicture", newImage);
                updateCommand.Parameters.AddWithValue("@NewDescription", newDescription);
                updateCommand.Parameters.AddWithValue("@NewIsHidden", newHiddenint);
                updateCommand.Parameters.AddWithValue("@CNP", cnp);
                updateCommand.ExecuteNonQuery();
            }
        }

        public List<string> userStocks()
        {
            List<string> stocks = new List<string>();

            string query = @"
    WITH UserStocks AS (
        SELECT STOCK_NAME
        FROM USER_STOCK
        WHERE USER_CNP = @UserCNP
    ),
    LatestStockValue AS (
        SELECT sv1.STOCK_NAME, sv1.PRICE
        FROM STOCK_VALUE sv1
        WHERE sv1.STOCK_NAME IN (SELECT STOCK_NAME FROM UserStocks)
          AND sv1.PRICE = (
              SELECT MAX(sv2.PRICE)
              FROM STOCK_VALUE sv2
              WHERE sv2.STOCK_NAME = sv1.STOCK_NAME
          )
    )
    SELECT 
        s.STOCK_SYMBOL,
        us.STOCK_NAME,
        us.QUANTITY,
        COALESCE(lsv.PRICE, 0) AS PRICE
    FROM USER_STOCK us
    JOIN STOCK s ON us.STOCK_NAME = s.STOCK_NAME
    LEFT JOIN LatestStockValue lsv ON s.STOCK_NAME = lsv.STOCK_NAME
    WHERE us.USER_CNP = @UserCNP;
    ";

            using (var command = new SqlCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@UserCNP", cnp);

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

        public string getLoggedInUserCNP()
        {
            return loggedInUserCNP;
        }

        //public List<string> userStocks()
        //{
        //    //string mySymbol;
        //    //string getSymquery = "SELECT STOCK_SYMBOL FROM [USER_STOCK] WHERE USER_CNP = @CNP";
        //    //using (var checkCommand = new SqlCommand(getSymquery, dbConnection))
        //    //{
        //    //    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
        //    //    if(checkCommand.ExecuteScalar() == null) mySymbol = string.Empty;
        //    //}

        //    //string myStockName;
        //    //string getStockNamequery = "SELECT NAME FROM [USER_STOCK] WHERE USER_CNP = @CNP";
        //    //using (var checkCommand = new SqlCommand(getStockNamequery, dbConnection))
        //    //{
        //    //    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
        //    //    if(checkCommand.ExecuteScalar() == null) myStockName = string.Empty;    
        //    //}

        //    //string myQuantity;
        //    //string getStockQuantituquery = "SELECT NAME FROM [USER_STOCK] WHERE USER_CNP = @CNP";
        //    //using (var checkCommand = new SqlCommand(getStockQuantituquery, dbConnection))
        //    //{
        //    //    checkCommand.Parameters.AddWithValue("@CNP", this.userCNP);
        //    //    if (checkCommand.ExecuteScalar() == null) { myQuantity = string.Empty; }    
        //    //}

        //    //stock LIST

        //    //get the user's stocks from the data base - make a string: logo + name + quantity + price
        //    //_user = new User("1234567890", "Caramel", "asdf", false, "https://static.wikia.nocookie.net/hellokitty/images/3/32/Sanrio_Characters_Keroppi_Image007.png/revision/latest/thumbnail/width/360/height/360?cb=20170405011801", false);
        //    //return new List<string> { "Stock A", "Stock B", "Stock C", "Stock D" };
        //   // return new List<string>();   
        //}
    }
}
