using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System;

namespace DataAccessLayer
{
    public class ConnectionManager
    {
        private readonly string? _connectionString;

        public ConnectionManager(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("DefaultConnection string is missing or empty in configuration.");
            }
        }
        public IDbConnection GetOpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}