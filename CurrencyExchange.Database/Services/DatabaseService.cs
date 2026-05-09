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
                WHERE  Username = @Username
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
    }
}
