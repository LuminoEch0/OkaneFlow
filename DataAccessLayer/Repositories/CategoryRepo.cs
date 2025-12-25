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
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ConnectionManager _dbManager;

        public CategoryRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public List<CategoryModel> GetCategories(Guid id)
        {
            var categories = new List<CategoryDTO>();
            string sql = "SELECT [CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed] FROM Category WHERE AccountID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new CategoryDTO
                            {
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("Name")),
                                AllocatedAmount = reader.GetDecimal(reader.GetOrdinal("AllocatedAmount")),
                                AmountUsed = reader.GetDecimal(reader.GetOrdinal("AmountUsed"))
                            });
                        }
                    }
                }
            }
            return CategoryMapper.ToModelList(categories);
        }

        public CategoryModel? GetCategoryById(Guid id)
        {
            CategoryDTO? dto = null;
            string sql = "SELECT [CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed] FROM Category WHERE CategoryID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dto = new CategoryDTO
                            {
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("Name")),
                                AllocatedAmount = reader.GetDecimal(reader.GetOrdinal("AllocatedAmount")),
                                AmountUsed = reader.GetDecimal(reader.GetOrdinal("AmountUsed"))
                            };
                            return CategoryMapper.ToModel(dto);
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateCategory(CategoryModel model)
        {
            // Map model to DTO for DB operations
            var dto = CategoryMapper.ToDTO(model);

            string sql = "UPDATE Category SET Name = @name, AllocatedAmount = @allocatedAmount, AmountUsed = @amountUsed WHERE CategoryID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", dto.CategoryID);
                    cmd.Parameters.AddWithValue("@name", dto.CategoryName);
                    cmd.Parameters.AddWithValue("@allocatedAmount", dto.AllocatedAmount);
                    cmd.Parameters.AddWithValue("@amountUsed", dto.AmountUsed);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCategory(Guid id)
        {
            string sql = "DELETE FROM Category WHERE CategoryID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateCategory(CategoryModel model)
        {
            // Ensure there is a CategoryID
            if (model.CategoryID == Guid.Empty)
            {
                model.CategoryID = Guid.NewGuid();
            }

            var dto = CategoryMapper.ToDTO(model);

            string sql = "INSERT INTO Category ([CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed]) VALUES (@categoryId, @accountId, @name, @allocatedAmount, @amountUsed)";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@categoryId", dto.CategoryID);
                    cmd.Parameters.AddWithValue("@accountId", dto.AccountID);
                    cmd.Parameters.AddWithValue("@name", dto.CategoryName);
                    cmd.Parameters.AddWithValue("@allocatedAmount", dto.AllocatedAmount);
                    cmd.Parameters.AddWithValue("@amountUsed", dto.AmountUsed);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AssignAmountAllocated(Guid categoryId, decimal amount)
        {
            string sql = "UPDATE Category SET AllocatedAmount = @allocatedamount WHERE CategoryID = @category";
            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@category", categoryId);
                    cmd.Parameters.AddWithValue("@allocatedamount", amount);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public CategoryModel GetUnassignedCategory(Guid accountId)
        {
            string sql = "SELECT [CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed] FROM Category WHERE AccountID = @accountId AND Name = 'Unassigned'";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var dto = new CategoryDTO
                            {
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("Name")),
                                AllocatedAmount = reader.GetDecimal(reader.GetOrdinal("AllocatedAmount")),
                                AmountUsed = reader.GetDecimal(reader.GetOrdinal("AmountUsed"))
                            };
                            return CategoryMapper.ToModel(dto);
                        }
                    }
                }
            }

            // If not found, create it as a model and persist
            var newModel = new CategoryModel(
                Guid.NewGuid(),
                accountId,
                "Unassigned",
                0m,
                0m);

            CreateCategory(newModel);
            return newModel;
        }
    }
}
