using System.Configuration;
using System.Data.SqlClient;

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
    }
}
