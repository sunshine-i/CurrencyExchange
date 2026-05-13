using CurrencyExchange.Database.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CurrencyExchange.Database.Services
{
    public class DatabaseService
    {
        private string ConnectionString =>
            ConfigurationManager.ConnectionStrings["CurrencyExchangeDb"].ConnectionString;

        private SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates a new user. Returns null if the username is already taken.
        /// </summary>
        public User RegisterUser(string username, string password)
        {
            const string sql = @"
                INSERT INTO Users (Username, PasswordHash)
                OUTPUT INSERTED.UserId, INSERTED.Username
                VALUES (@Username, @Hash)";

            try
            {
                using (var conn = OpenConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    cmd.Parameters.Add("@Hash", SqlDbType.NVarChar, 64).Value = HashPassword(password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return new User
                            {
                                UserId = (int)reader["UserId"],
                                Username = (string)reader["Username"]
                            };
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // unique constraint violation
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// Validates credentials. Returns the User on success, null on failure.
        /// </summary>
        public User Login(string username, string password)
        {
            const string sql = @"
                SELECT UserId, Username
                FROM   Users
                WHERE  Username     = @Username
                AND    PasswordHash = @Hash";

            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                cmd.Parameters.Add("@Hash", SqlDbType.NVarChar, 64).Value = HashPassword(password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return new User
                        {
                            UserId = (int)reader["UserId"],
                            Username = (string)reader["Username"]
                        };
                }
            }

            return null;
        }

        // ------------------------------------------------------------------ //
        //  Balances
        // ------------------------------------------------------------------ //

        public List<Balance> GetBalances(int userId)
        {
            const string sql = @"
                SELECT CurrencyCode, Amount
                FROM   Balances
                WHERE  UserId = @UserId
                ORDER BY CurrencyCode";

            var list = new List<Balance>();

            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(new Balance
                        {
                            CurrencyCode = (string)reader["CurrencyCode"],
                            Amount = (double)reader["Amount"]
                        });
                }
            }

            return list;
        }

        public double GetBalance(int userId, string currencyCode)
        {
            const string sql = @"
                SELECT ISNULL(Amount, 0)
                FROM   Balances
                WHERE  UserId = @UserId AND CurrencyCode = @Code";

            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 10).Value = currencyCode;

                var result = cmd.ExecuteScalar();
                return result == null ? 0.0 : (double)result;
            }
        }

        /// <summary>
        /// Adds (or subtracts, if delta is negative) to a currency balance.
        /// Uses MERGE so the row is created if it doesn't exist yet.
        /// </summary>
        public void AdjustBalance(SqlConnection conn, SqlTransaction tx,
                                   int userId, string currencyCode, double delta)
        {
            const string sql = @"
                MERGE Balances AS target
                USING (SELECT @UserId AS UserId, @Code AS CurrencyCode) AS source
                ON target.UserId = source.UserId AND target.CurrencyCode = source.CurrencyCode
                WHEN MATCHED THEN
                    UPDATE SET Amount = Amount + @Delta
                WHEN NOT MATCHED THEN
                    INSERT (UserId, CurrencyCode, Amount)
                    VALUES (@UserId, @Code, @Delta);";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 10).Value = currencyCode;
                cmd.Parameters.Add("@Delta", SqlDbType.Float).Value = delta;
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------------------------------------------------------ //
        //  Transactions
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Records a top-up and increases the balance — both in one DB transaction.
        /// </summary>
        public void TopUp(int userId, string currencyCode, double amount)
        {
            const string insertTx = @"
                INSERT INTO Transactions (UserId, Type, ToCurrency, ToAmount, Timestamp)
                VALUES (@UserId, 'TOPUP', @Currency, @Amount, GETDATE())";

            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    AdjustBalance(conn, tx, userId, currencyCode, amount);

                    using (var cmd = new SqlCommand(insertTx, conn, tx))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@Currency", SqlDbType.NVarChar, 10).Value = currencyCode;
                        cmd.Parameters.Add("@Amount", SqlDbType.Float).Value = amount;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Records an exchange, deducts FromCurrency and credits ToCurrency — one DB transaction.
        /// Throws InvalidOperationException if balance is insufficient.
        /// </summary>
        public void RecordExchange(int userId, string fromCurrency, string toCurrency,
                                   double fromAmount, double toAmount, string rate)
        {
            const string insertTx = @"
                INSERT INTO Transactions
                    (UserId, Type, FromCurrency, ToCurrency, FromAmount, ToAmount, Rate, Timestamp)
                VALUES
                    (@UserId, 'EXCHANGE', @From, @To, @FromAmt, @ToAmt, @Rate, GETDATE())";

            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    // Re-check balance inside the transaction to guard against race conditions
                    double currentBalance = 0;
                    const string checkSql =
                        "SELECT ISNULL(Amount, 0) FROM Balances WHERE UserId = @UserId AND CurrencyCode = @Code";

                    using (var cmd = new SqlCommand(checkSql, conn, tx))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 10).Value = fromCurrency;
                        var result = cmd.ExecuteScalar();
                        currentBalance = result == null ? 0.0 : (double)result;
                    }

                    if (currentBalance < fromAmount)
                        throw new InvalidOperationException(
                            $"Insufficient balance. You have {currentBalance:N4} {fromCurrency}, need {fromAmount:N4}.");

                    AdjustBalance(conn, tx, userId, fromCurrency, -fromAmount);
                    AdjustBalance(conn, tx, userId, toCurrency, toAmount);

                    using (var cmd = new SqlCommand(insertTx, conn, tx))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@From", SqlDbType.NVarChar, 10).Value = fromCurrency;
                        cmd.Parameters.Add("@To", SqlDbType.NVarChar, 10).Value = toCurrency;
                        cmd.Parameters.Add("@FromAmt", SqlDbType.Float).Value = fromAmount;
                        cmd.Parameters.Add("@ToAmt", SqlDbType.Float).Value = toAmount;
                        cmd.Parameters.Add("@Rate", SqlDbType.NVarChar, 50).Value = rate;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public List<Transaction> GetTransactions(int userId)
        {
            const string sql = @"
                SELECT TransactionId, Type, FromCurrency, ToCurrency,
                       FromAmount, ToAmount, Rate, Timestamp
                FROM   Transactions
                WHERE  UserId = @UserId
                ORDER BY Timestamp DESC";

            var list = new List<Transaction>();

            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Transaction
                        {
                            TransactionId = (int)reader["TransactionId"],
                            Type = (string)reader["Type"],
                            FromCurrency = reader["FromCurrency"] as string,
                            ToCurrency = reader["ToCurrency"] as string,
                            FromAmount = reader["FromAmount"] as double?,
                            ToAmount = reader["ToAmount"] as double?,
                            Rate = reader["Rate"] as string,
                            Timestamp = (DateTime)reader["Timestamp"]
                        });
                    }
                }
            }

            return list;
        }
    }
}