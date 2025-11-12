using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace OkaneFlow.Data
{
    // A simple interface to allow dependency injection (DI) in C#
    public interface IDbConnectionManager
    {
        IDbConnection GetOpenConnection();
    }

    // Concrete implementation for MSSQL
    public class ConnectionManager : IDbConnectionManager
    {
        private readonly string? _connectionString;

        // IConfiguration is typically injected by the C# framework
        public ConnectionManager(IConfiguration configuration)
        {
            // IMPORTANT: The connection string must be stored in appsettings.json
            // Example: "ConnectionStrings": { "DefaultConnection": "Server=..." }
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("DefaultConnection string is not configured.");
            }
        }

        /// <summary>
        /// Provides an open SqlConnection object. The caller MUST handle disposal (using statement).
        /// </summary>
        /// <returns>An open IDbConnection object.</returns>
        public IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                return connection;
            }
            catch (SqlException ex)
            {
                // Log the exception details here
                Console.WriteLine($"Database connection failed: {ex.Message}");
                // Re-throw a generic exception to avoid exposing SQL details to the UI
                throw new ApplicationException("Could not connect to the database.");
            }
        }
    }
}
