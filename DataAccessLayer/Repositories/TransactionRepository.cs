using DataAccessLayer.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class TransactionRepository
    {
        private readonly ConnectionManager _dbManager;

        public TransactionRepository(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public List<TransactionDTO> GetTransactionsByAccountId(Guid accountId)
        {
            var transactions = new List<TransactionDTO>();
            // Join with Category to filter by AccountID
            string sql = @"
                SELECT t.TransactionID, t.CategoryID, t.Amount, t.Description, t.Date, t.Type 
                FROM [Transaction] t
                INNER JOIN Category c ON t.CategoryID = c.CategoryID
                WHERE c.AccountID = @accountId
                ORDER BY t.Date DESC";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@accountId", accountId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new TransactionDTO
                            {
                                TransactionID = reader.GetGuid(reader.GetOrdinal("TransactionID")),
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Type = reader.GetInt32(reader.GetOrdinal("Type"))
                            });
                        }
                    }
                }
            }
            return transactions;
        }

        public void AddTransaction(TransactionDTO transaction)
        {
            string sql = @"
                INSERT INTO [Transaction] (TransactionID, CategoryID, Amount, Description, Date, Type) 
                VALUES (@transactionId, @categoryId, @amount, @description, @date, @type)";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@transactionId", transaction.TransactionID);
                    command.Parameters.AddWithValue("@categoryId", transaction.CategoryID);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@description", (object)transaction.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@date", transaction.Date);
                    command.Parameters.AddWithValue("@type", transaction.Type);

                    command.ExecuteNonQuery();
                }
            }
        }

        public TransactionDTO? GetTransactionById(Guid id)
        {
            string sql = "SELECT TransactionID, CategoryID, Amount, Description, Date, Type FROM [Transaction] WHERE TransactionID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TransactionDTO
                            {
                                TransactionID = reader.GetGuid(reader.GetOrdinal("TransactionID")),
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Type = reader.GetInt32(reader.GetOrdinal("Type"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateTransaction(TransactionDTO transaction)
        {
            string sql = @"
                UPDATE [Transaction] 
                SET CategoryID = @categoryId, Amount = @amount, Description = @description, Date = @date, Type = @type 
                WHERE TransactionID = @transactionId";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@transactionId", transaction.TransactionID);
                    command.Parameters.AddWithValue("@categoryId", transaction.CategoryID);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@description", (object)transaction.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@date", transaction.Date);
                    command.Parameters.AddWithValue("@type", transaction.Type);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTransaction(Guid id)
        {
            string sql = "DELETE FROM [Transaction] WHERE TransactionID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
