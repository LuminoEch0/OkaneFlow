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

        public async Task<List<CategoryModel>> GetCategoriesAsync(Guid id)
        {
            var categories = new List<CategoryDTO>();
            string sql = "SELECT [CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed] FROM Category WHERE AccountID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

        public async Task<CategoryModel?> GetCategoryByIdAsync(Guid id)
        {
            CategoryDTO? dto = null;
            string sql = "SELECT [CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed] FROM Category WHERE CategoryID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
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

        public async Task UpdateCategoryAsync(CategoryModel model)
        {
            var dto = CategoryMapper.ToDTO(model);

            string sql = "UPDATE Category SET Name = @name, AllocatedAmount = @allocatedAmount, AmountUsed = @amountUsed WHERE CategoryID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", dto.CategoryID);
                    cmd.Parameters.AddWithValue("@name", dto.CategoryName);
                    cmd.Parameters.AddWithValue("@allocatedAmount", dto.AllocatedAmount);
                    cmd.Parameters.AddWithValue("@amountUsed", dto.AmountUsed);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            string sql = "DELETE FROM Category WHERE CategoryID = @id";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CreateCategoryAsync(CategoryModel model)
        {
            if (model.CategoryID == Guid.Empty)
            {
                model.CategoryID = Guid.NewGuid();
            }

            var dto = CategoryMapper.ToDTO(model);

            string sql = "INSERT INTO Category ([CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed]) VALUES (@categoryId, @accountId, @name, @allocatedAmount, @amountUsed)";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@categoryId", dto.CategoryID);
                    cmd.Parameters.AddWithValue("@accountId", dto.AccountID);
                    cmd.Parameters.AddWithValue("@name", dto.CategoryName);
                    cmd.Parameters.AddWithValue("@allocatedAmount", dto.AllocatedAmount);
                    cmd.Parameters.AddWithValue("@amountUsed", dto.AmountUsed);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AssignAmountAllocatedAsync(Guid categoryId, decimal amount)
        {
            string sql = "UPDATE Category SET AllocatedAmount = @allocatedamount WHERE CategoryID = @category";
            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@category", categoryId);
                    cmd.Parameters.AddWithValue("@allocatedamount", amount);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }



        public async Task<CategoryModel> GetUnallocatedCategoryAsync(Guid accountId)
        {
            string sql = "SELECT [CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed] FROM Category WHERE AccountID = @accountId AND Name = 'Unallocated'";

            using (IDbConnection connection = await _dbManager.GetOpenConnectionAsync())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
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

            var newModel = new CategoryModel(
                Guid.NewGuid(),
                accountId,
                "Unallocated",
                0m,
                0m);

            await CreateCategoryAsync(newModel);
            return newModel;
        }
    }
}
