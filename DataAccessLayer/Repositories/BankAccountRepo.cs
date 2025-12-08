using DataAccessLayer.DataTransferObjects;
using DataAccessLayer.Repositories.Interface;
using Microsoft.Data.SqlClient;
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
        public List<BankAccountDTO> GetBankAccounts(Guid id)
        {
            var bankAccounts = new List<BankAccountDTO>();
            string sql = "SELECT [AccountID],[UserID],[AccountName],[CurrentBalance] FROM BankAccount WHERE [UserID] = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
            return bankAccounts;
        }

        public BankAccountDTO? GetBankAccountById(Guid id)
        {
            string sql = "SELECT [AccountID],[UserID],[AccountName],[CurrentBalance] FROM BankAccount WHERE AccountID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BankAccountDTO
                            {
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                UserID = reader.GetGuid(reader.GetOrdinal("UserID")),
                                AccountName = reader.GetString(reader.GetOrdinal("AccountName")),
                                CurrentBalance = reader.GetDecimal(reader.GetOrdinal("CurrentBalance"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateBankAccount(BankAccountDTO dto)
        {
            string sql = "UPDATE BankAccount SET AccountName = @name, CurrentBalance = @balance WHERE AccountID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", dto.AccountID);
                    cmd.Parameters.AddWithValue("@balance", dto.CurrentBalance);
                    cmd.Parameters.AddWithValue("@name", dto.AccountName);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBankAccount(Guid id)
        {
            string sql = "DELETE FROM BankAccount WHERE AccountID = @id";
            
            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    // Repository accepts the ID, not the DTO
                    cmd.Parameters.AddWithValue("@id", id);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateBankAccount(BankAccountDTO newAccount)
        {
            string sql = "INSERT INTO BankAccount ([AccountID],[UserID],[AccountName],[CurrentBalance]) VALUES (@id, @userId, @name, @balance)";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", newAccount.AccountID);
                    cmd.Parameters.AddWithValue("@userId", newAccount.UserID);
                    cmd.Parameters.AddWithValue("@name", newAccount.AccountName);
                    cmd.Parameters.AddWithValue("@balance", newAccount.CurrentBalance);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}