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
    public class SubscriptionRepo : ISubscriptionRepo
    {
        private readonly ConnectionManager _dbManager;

        public SubscriptionRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public List<SubscriptionModel> GetSubscriptions(Guid accountId)
        {
            var subscriptions = new List<SubscriptionDTO>();
            string sql = "SELECT [SubscriptionID],[AccountID],[CategoryID],[Name],[Amount],[Frequency],[StartDate],[Description] FROM [Subscription] WHERE AccountID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", accountId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subscriptions.Add(new SubscriptionDTO
                            {
                                SubscriptionID = reader.GetGuid(reader.GetOrdinal("SubscriptionID")),
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                Frequency = reader.GetString(reader.GetOrdinal("Frequency")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                            });
                        }
                    }
                }
            }
            return SubscriptionMapper.ToModelList(subscriptions);
        }

        public SubscriptionModel? GetSubscriptionById(Guid id)
        {
            SubscriptionDTO? dto = null;
            string sql = "SELECT [SubscriptionID],[AccountID],[CategoryID],[Name],[Amount],[Frequency],[StartDate],[Description] FROM Subscription WHERE SubscriptionID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dto = new SubscriptionDTO
                            {
                                SubscriptionID = reader.GetGuid(reader.GetOrdinal("SubscriptionID")),
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                Frequency = reader.GetString(reader.GetOrdinal("Frequency")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                            };
                            return SubscriptionMapper.ToModel(dto);
                        }
                    }
                }
            }
            return null;
        }

        public void CreateSubscription(SubscriptionModel model)
        {
            if (model.SubscriptionID == Guid.Empty)
            {
                model.SubscriptionID = Guid.NewGuid();
            }

            var dto = SubscriptionMapper.ToDTO(model);

            string sql = "INSERT INTO Subscription ([SubscriptionID],[AccountID],[CategoryID],[Name],[Amount],[Frequency],[StartDate],[Description]) VALUES (@subscriptionId, @accountId, @categoryId, @name, @amount, @frequency, @startDate, @description)";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@subscriptionId", dto.SubscriptionID);
                    cmd.Parameters.AddWithValue("@accountId", dto.AccountID);
                    cmd.Parameters.AddWithValue("@categoryId", dto.CategoryID);
                    cmd.Parameters.AddWithValue("@name", dto.Name);
                    cmd.Parameters.AddWithValue("@amount", dto.Amount);
                    cmd.Parameters.AddWithValue("@frequency", dto.Frequency);
                    cmd.Parameters.AddWithValue("@startDate", dto.StartDate);
                    cmd.Parameters.AddWithValue("@description", dto.Description ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateSubscription(SubscriptionModel model)
        {
            var dto = SubscriptionMapper.ToDTO(model);

            string sql = "UPDATE Subscription SET Name = @name, Amount = @amount, Frequency = @frequency, StartDate = @startDate, Description = @description, CategoryID = @categoryId WHERE SubscriptionID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", dto.SubscriptionID);
                    cmd.Parameters.AddWithValue("@name", dto.Name);
                    cmd.Parameters.AddWithValue("@amount", dto.Amount);
                    cmd.Parameters.AddWithValue("@frequency", dto.Frequency);
                    cmd.Parameters.AddWithValue("@startDate", dto.StartDate);
                    cmd.Parameters.AddWithValue("@description", dto.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@categoryId", dto.CategoryID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSubscription(Guid id)
        {
            string sql = "DELETE FROM Subscription WHERE SubscriptionID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
