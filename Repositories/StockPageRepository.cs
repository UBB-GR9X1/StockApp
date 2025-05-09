namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    /// <summary>
    /// Repository for managing stock page data, including user and stock information.
    /// </summary>
    public class StockPageRepository : IStockPageRepository
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Gets the user associated with the stock page, or <c>null</c> if guest.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current user is a guest.
        /// </summary>
        public bool IsGuest { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockPageRepository"/> class,
        /// fetching the current user's CNP and loading their profile.
        /// </summary>
        public StockPageRepository(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.User = null;
            this.IsGuest = true;
            this.InitializeUser();
        }

        /// <summary>
        /// Updates the user's gem balance in database and memory.
        /// </summary>
        /// <param name="gems">New gem balance to set.</param>
        public void UpdateUserGems(int newGemBalance)
        {
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand("UPDATE [USER] SET GEM_BALANCE = @gems WHERE CNP = @cnp", connection);
            command.Parameters.AddWithValue("@gems", newGemBalance);
            command.Parameters.AddWithValue("@cnp", IUserRepository.CurrentUserCNP);
            command.ExecuteNonQuery();

            // Inline: reflect change in in-memory User object
            if (!IUserRepository.IsGuest)
            {
                this.User.GemBalance = newGemBalance;
            }
        }

        /// <summary>
        /// Adds or updates the quantity of a user's stock holding.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="quantity">Quantity to add to existing holdings.</param>
        public void AddOrUpdateUserStock(string stockName, int quantity)
        {
            const string query = @"
                IF EXISTS (SELECT 1 FROM USER_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name)
                BEGIN
                    UPDATE USER_STOCK SET QUANTITY = QUANTITY + @quantity WHERE USER_CNP = @cnp AND STOCK_NAME = @name
                END
                ELSE
                BEGIN
                    INSERT INTO USER_STOCK (USER_CNP, STOCK_NAME, QUANTITY) VALUES (@cnp, @name, @quantity)
                END";
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@cnp", this.User?.CNP);
            command.Parameters.AddWithValue("@name", stockName);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts a new stock price record.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="price">Price to record.</param>
        public void AddStockValue(string stockName, int price)
        {
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand("INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@name, @price)", connection);
            command.Parameters.AddWithValue("@name", stockName);
            command.Parameters.AddWithValue("@price", price);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves a <see cref="Stock"/> by name, including the latest price and user quantity.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns>A <see cref="Stock"/> instance with populated fields.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the stock is not found.</exception>
        public Stock GetStock(string stockName)
        {
            if (IUserRepository.IsGuest)
            {
                throw new Exception("User is not logged in.");
            }

            const string query = @"
                SELECT s.STOCK_NAME, s.STOCK_SYMBOL, s.AUTHOR_CNP, sv.PRICE, us.QUANTITY
                FROM STOCK s
                INNER JOIN STOCK_VALUE sv ON s.STOCK_NAME = sv.STOCK_NAME
                LEFT JOIN USER_STOCK us ON s.STOCK_NAME = us.STOCK_NAME AND us.USER_CNP = @cnp
                WHERE s.STOCK_NAME = @name";
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", stockName);
            command.Parameters.AddWithValue("@cnp", IUserRepository.CurrentUserCNP);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                // Inline: map fields to Stock constructor
                return new Stock(
                    name: reader["STOCK_NAME"].ToString() ?? throw new Exception("Stock name not found."),
                    symbol: reader["STOCK_SYMBOL"].ToString() ?? throw new Exception("Stock symbol not found."),
                    authorCNP: reader["AUTHOR_CNP"].ToString() ?? throw new Exception("Author CNP not found."),
                    price: Convert.ToInt32(reader["PRICE"]),
                    quantity: reader["QUANTITY"] != DBNull.Value ? Convert.ToInt32(reader["QUANTITY"]) : 0);
            }

            // FIXME: Consider returning null instead of throwing to simplify client logic
            throw new InvalidOperationException($"Stock with name '{stockName}' not found.");
        }

        /// <summary>
        /// Retrieves the full price history for a given stock.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns>List of historical prices.</returns>
        public List<int> GetStockHistory(string stockName)
        {
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand("SELECT PRICE FROM STOCK_VALUE WHERE STOCK_NAME = @name", connection);
            command.Parameters.AddWithValue("@name", stockName);

            using var reader = command.ExecuteReader();
            var stockValues = new List<int>();
            while (reader.Read())
            {
                stockValues.Add(Convert.ToInt32(reader["PRICE"]));
            }

            return stockValues;
        }

        /// <summary>
        /// Retrieves the quantity of stocks owned by the user.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns>Quantity owned.</returns>
        public int GetOwnedStocks(string stockName)
        {
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand("SELECT QUANTITY FROM USER_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", connection);
            command.Parameters.AddWithValue("@cnp", this.User?.CNP);
            command.Parameters.AddWithValue("@name", stockName);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Convert.ToInt32(reader["QUANTITY"]) : 0;
        }

        /// <summary>
        /// Checks if the specified stock is in the user's favorites.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns><c>true</c> if favorite; otherwise, <c>false</c>.</returns>
        public bool GetFavorite(string stockName)
        {
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand("SELECT 1 FROM FAVORITE_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", connection);
            command.Parameters.AddWithValue("@cnp", IUserRepository.CurrentUserCNP);
            command.Parameters.AddWithValue("@name", stockName);

            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        /// <summary>
        /// Toggles the favorite status of a stock for the user.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="state"><c>true</c> to add favorite; <c>false</c> to remove.</param>
        public void ToggleFavorite(string stockName, bool state)
        {
            if (state)
            {
                using SqlConnection connection = DatabaseHelper.GetConnection();
                using var command = new SqlCommand("INSERT INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME) VALUES (@cnp, @name)", connection);
                command.Parameters.AddWithValue("@cnp", this.User?.CNP);
                command.Parameters.AddWithValue("@name", stockName);
                command.ExecuteNonQuery();
            }
            else
            {
                using SqlConnection connection = DatabaseHelper.GetConnection();
                using var command = new SqlCommand("DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", connection);
                command.Parameters.AddWithValue("@cnp", this.User?.CNP);
                command.Parameters.AddWithValue("@name", stockName);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Initializes the <see cref="User"/> property based on the CNP; sets <see cref="IsGuest"/> accordingly.
        /// </summary>
        private void InitializeUser()
        {
            using SqlConnection connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(
                @"
               SELECT 
                   Id, Cnp, FirstName, LastName, Email, PhoneNumber, HashedPassword, 
                   NumberOfOffenses, RiskScore, ROI, CreditScore, Birthday, 
                   ZodiacSign, ZodiacAttribute, NumberOfBillSharesPaid, Income, Balance
               FROM Users 
               WHERE Cnp = @cnp", connection);
            command.Parameters.AddWithValue("@cnp", IUserRepository.CurrentUserCNP);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                this.User = new User(
                    id: reader.GetInt32(reader.GetOrdinal("Id")),
                    cnp: reader.GetString(reader.GetOrdinal("Cnp")),
                    firstName: reader.GetString(reader.GetOrdinal("FirstName")),
                    lastName: reader.GetString(reader.GetOrdinal("LastName")),
                    email: reader.GetString(reader.GetOrdinal("Email")),
                    phoneNumber: reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    hashedPassword: reader.IsDBNull(reader.GetOrdinal("HashedPassword")) ? null : reader.GetString(reader.GetOrdinal("HashedPassword")),
                    numberOfOffenses: reader.IsDBNull(reader.GetOrdinal("NumberOfOffenses")) ? null : reader.GetInt32(reader.GetOrdinal("NumberOfOffenses")),
                    riskScore: reader.IsDBNull(reader.GetOrdinal("RiskScore")) ? null : reader.GetInt32(reader.GetOrdinal("RiskScore")),
                    roi: reader.IsDBNull(reader.GetOrdinal("ROI")) ? null : reader.GetDecimal(reader.GetOrdinal("ROI")),
                    creditScore: reader.IsDBNull(reader.GetOrdinal("CreditScore")) ? null : reader.GetInt32(reader.GetOrdinal("CreditScore")),
                    birthday: reader.IsDBNull(reader.GetOrdinal("Birthday")) ? null : reader.GetDateTime(reader.GetOrdinal("Birthday")).Date,
                    zodiacSign: reader.IsDBNull(reader.GetOrdinal("ZodiacSign")) ? null : reader.GetString(reader.GetOrdinal("ZodiacSign")),
                    zodiacAttribute: reader.IsDBNull(reader.GetOrdinal("ZodiacAttribute")) ? null : reader.GetString(reader.GetOrdinal("ZodiacAttribute")),
                    numberOfBillSharesPaid: reader.GetInt32(reader.GetOrdinal("NumberOfBillSharesPaid")),
                    income: reader.GetInt32(reader.GetOrdinal("Income")),
                    balance: reader.GetDecimal(reader.GetOrdinal("Balance"))
                );
                this.IsGuest = false;
            }
            else
            {
                this.User = null;
                this.IsGuest = true;
            }
        }
    }
}
