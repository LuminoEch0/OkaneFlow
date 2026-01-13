using DataAccessLayer.DataTransferObjects;
using DataAccessLayer.Mappers;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Service.Models;
using Service.RepoInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class BankAccountRepo : IBankAccountRepo
    {
        private readonly ConnectionManager _dbManager;

        public BankAccountRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }
        public async Task<List<BankAccountModel>> GetBankAccountsAsync(Guid id)
        {
            var bankAccounts = new List<BankAccountDTO>();
            string sql = "SELECT [AccountID],[UserID],[AccountName],[CurrentBalance] FROM BankAccount WHERE [UserID] = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bankAccounts.Add(new BankAccountDTO
                            {
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                UserID = reader.GetGuid(reader.GetOrdinal("UserID")),
                                AccountName = reader.GetString(reader.GetOrdinal("AccountName")),
                                CurrentBalance = reader.GetDecimal(reader.GetOrdinal("CurrentBalance"))
                            });
                        }
                    }
                }
            }
            return BankAccountMapper.ToModelList(bankAccounts);
        }

        public async Task<BankAccountModel?> GetBankAccountByIdAsync(Guid id)
        {
            string sql = "SELECT [AccountID],[UserID],[AccountName],[CurrentBalance] FROM BankAccount WHERE AccountID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new BankAccountModel
                            (
                                reader.GetGuid(reader.GetOrdinal("AccountID")),
                                reader.GetGuid(reader.GetOrdinal("UserID")),
                                reader.GetString(reader.GetOrdinal("AccountName")),
                                reader.GetDecimal(reader.GetOrdinal("CurrentBalance"))
                            );

                        }
                        ;
                    }
                }
            }
            return null;
        }


        public async Task UpdateBankAccountAsync(BankAccountModel model)
        {
            string sql = "UPDATE BankAccount SET AccountName = @name, CurrentBalance = @balance WHERE AccountID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", model.AccountID);
                    cmd.Parameters.AddWithValue("@balance", model.CurrentBalance);
                    cmd.Parameters.AddWithValue("@name", model.AccountName);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteBankAccountAsync(Guid id)
        {
            string sql = "DELETE FROM BankAccount WHERE AccountID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CreateBankAccountAsync(BankAccountModel newAccount)
        {
            string sql = "INSERT INTO BankAccount ([AccountID],[UserID],[AccountName],[CurrentBalance]) VALUES (@id, @userId, @name, @balance)";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", newAccount.AccountID);
                    cmd.Parameters.AddWithValue("@userId", newAccount.UserID);
                    cmd.Parameters.AddWithValue("@name", newAccount.AccountName);
                    cmd.Parameters.AddWithValue("@balance", newAccount.CurrentBalance);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}