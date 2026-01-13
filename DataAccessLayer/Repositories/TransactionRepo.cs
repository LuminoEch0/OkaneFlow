using DataAccessLayer.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Service.RepoInterface;
using Service.Models;
using DataAccessLayer.Mappers;

namespace DataAccessLayer.Repositories
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly ConnectionManager _dbManager;

        public TransactionRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<TransactionModel>> GetTransactionsByAccountIdAsync(Guid accountId)
        {
            var transactions = new List<TransactionDTO>();
            // Join with Category to filter by AccountID
            string sql = @"
            SELECT [TransactionID],[Transaction].[CategoryID],[Amount],[Description],[Date],[Type] FROM [Transaction]
            INNER JOIN [Category] 
                ON [Transaction].[CategoryID] = [Category].[CategoryID]
            WHERE [Category].[AccountID] = @accountId
            ORDER BY [Transaction].[Date] DESC;
        ";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@accountId", accountId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
            return transactions.Select(TransactionMapper.ToModel).ToList();
        }

        public async Task AddTransactionAsync(TransactionModel transaction)
        {
            var dto = TransactionMapper.ToDTO(transaction);
            string sql = @"
                INSERT INTO [Transaction] (TransactionID, CategoryID, Amount, Description, Date, Type) 
                VALUES (@transactionId, @categoryId, @amount, @description, @date, @type)";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@transactionId", dto.TransactionID);
                    command.Parameters.AddWithValue("@categoryId", dto.CategoryID);
                    command.Parameters.AddWithValue("@amount", dto.Amount);
                    command.Parameters.AddWithValue("@description", (object?)dto.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@date", dto.Date);
                    command.Parameters.AddWithValue("@type", dto.Type);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<TransactionModel?> GetTransactionByIdAsync(Guid id)
        {
            string sql = "SELECT TransactionID, CategoryID, Amount, Description, Date, Type FROM [Transaction] WHERE TransactionID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var dto = new TransactionDTO
                            {
                                TransactionID = reader.GetGuid(reader.GetOrdinal("TransactionID")),
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Type = reader.GetInt32(reader.GetOrdinal("Type"))
                            };
                            return TransactionMapper.ToModel(dto);
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateTransactionAsync(TransactionModel transaction)
        {
            var dto = TransactionMapper.ToDTO(transaction);
            string sql = @"
                UPDATE [Transaction] 
                SET CategoryID = @categoryId, Amount = @amount, Description = @description, Date = @date, Type = @type 
                WHERE TransactionID = @transactionId";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@transactionId", dto.TransactionID);
                    command.Parameters.AddWithValue("@categoryId", dto.CategoryID);
                    command.Parameters.AddWithValue("@amount", dto.Amount);
                    command.Parameters.AddWithValue("@description", (object?)dto.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@date", dto.Date);
                    command.Parameters.AddWithValue("@type", dto.Type);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteTransactionAsync(Guid id)
        {
            string sql = "DELETE FROM [Transaction] WHERE TransactionID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateCategoryForTransactionsAsync(Guid oldCategoryId, Guid newCategoryId, int expenseTypeId)
        {
            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                string moveSql = "UPDATE [Transaction] SET CategoryID = @newCategoryId WHERE CategoryID = @oldCategoryId";
                using (var moveCmd = new SqlCommand(moveSql, (SqlConnection)connection))
                {
                    moveCmd.Parameters.AddWithValue("@oldCategoryId", oldCategoryId);
                    moveCmd.Parameters.AddWithValue("@newCategoryId", newCategoryId);
                    await moveCmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
