using DataAccessLayer.DataTransferObjects;
using Service.RepoInterface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Service.Models;
using DataAccessLayer.Mappers;

namespace DataAccessLayer.Repositories
{
    public class DebtRepo : IDebtRepo
    {
        private readonly ConnectionManager _dbManager;

        public DebtRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<DebtModel>> GetDebtsAsync(Guid userId)
        {
            var debts = new List<DebtDTO>();
            string sql = "SELECT [DebtID],[UserID],[AccountID],[Name],[AssociatedEntity],[InitialAmount],[RemainingAmount],[IsInterestEnabled],[InterestRate],[DueDate],[Type] FROM Debt WHERE UserID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", userId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            debts.Add(MapReaderToDTO(reader));
                        }
                    }
                }
            }
            return DebtMapper.ToModelList(debts);
        }

        public async Task<DebtModel?> GetDebtByIdAsync(Guid id)
        {
            string sql = "SELECT [DebtID],[UserID],[AccountID],[Name],[AssociatedEntity],[InitialAmount],[RemainingAmount],[IsInterestEnabled],[InterestRate],[DueDate],[Type] FROM Debt WHERE DebtID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return DebtMapper.ToModel(MapReaderToDTO(reader));
                        }
                    }
                }
            }
            return null;
        }

        public async Task CreateDebtAsync(DebtModel model)
        {
            if (model.DebtID == Guid.Empty)
            {
                model.DebtID = Guid.NewGuid();
            }

            var dto = DebtMapper.ToDTO(model);

            string sql = "INSERT INTO Debt ([DebtID],[UserID],[AccountID],[Name],[AssociatedEntity],[InitialAmount],[RemainingAmount],[IsInterestEnabled],[InterestRate],[DueDate],[Type]) VALUES (@debtId, @userId, @accountId, @name, @associatedEntity, @initialAmount, @remainingAmount, @isInterestEnabled, @interestRate, @dueDate, @type)";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    AddParameters(cmd, dto);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateDebtAsync(DebtModel model)
        {
            var dto = DebtMapper.ToDTO(model);

            string sql = "UPDATE Debt SET UserID = @userId, AccountID = @accountId, Name = @name, AssociatedEntity = @associatedEntity, InitialAmount = @initialAmount, RemainingAmount = @remainingAmount, IsInterestEnabled = @isInterestEnabled, InterestRate = @interestRate, DueDate = @dueDate, Type = @type WHERE DebtID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    AddParameters(cmd, dto);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteDebtAsync(Guid id)
        {
            string sql = "DELETE FROM Debt WHERE DebtID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private DebtDTO MapReaderToDTO(SqlDataReader reader)
        {
            return new DebtDTO
            {
                DebtID = reader.GetGuid(reader.GetOrdinal("DebtID")),
                UserID = reader.GetGuid(reader.GetOrdinal("UserID")),
                AccountID = reader.IsDBNull(reader.GetOrdinal("AccountID")) ? null : reader.GetGuid(reader.GetOrdinal("AccountID")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                AssociatedEntity = reader.GetString(reader.GetOrdinal("AssociatedEntity")),
                InitialAmount = reader.GetDecimal(reader.GetOrdinal("InitialAmount")),
                RemainingAmount = reader.GetDecimal(reader.GetOrdinal("RemainingAmount")),
                IsInterestEnabled = reader.GetBoolean(reader.GetOrdinal("IsInterestEnabled")),
                InterestRate = reader.GetDecimal(reader.GetOrdinal("InterestRate")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                Type = reader.GetInt32(reader.GetOrdinal("Type"))
            };
        }

        private void AddParameters(SqlCommand cmd, DebtDTO dto)
        {
            cmd.Parameters.AddWithValue("@debtId", dto.DebtID);
            cmd.Parameters.AddWithValue("@userId", dto.UserID);
            cmd.Parameters.AddWithValue("@accountId", (object?)dto.AccountID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", dto.Name);
            cmd.Parameters.AddWithValue("@associatedEntity", dto.AssociatedEntity);
            cmd.Parameters.AddWithValue("@initialAmount", dto.InitialAmount);
            cmd.Parameters.AddWithValue("@remainingAmount", dto.RemainingAmount);
            cmd.Parameters.AddWithValue("@isInterestEnabled", dto.IsInterestEnabled);
            cmd.Parameters.AddWithValue("@interestRate", dto.InterestRate);
            cmd.Parameters.AddWithValue("@dueDate", dto.DueDate);
            cmd.Parameters.AddWithValue("@type", dto.Type);

            if (!cmd.Parameters.Contains("@id"))
            {
                cmd.Parameters.AddWithValue("@id", dto.DebtID);
            }
        }
    }
}
