using DataAccessLayer.DataTransferObjects;
using DataAccessLayer.Repositories.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepo : ICategoryRepo
    {
        //private string name = "@categoryName";
        private readonly ConnectionManager _dbManager;

        public CategoryRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }
        public List<CategoryDTO> GetCategories(Guid id)
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
            return categories;
        }

        public CategoryDTO? GetCategoryById(Guid id)
        {
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
                            return new CategoryDTO
                            {
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("Name")),
                                AllocatedAmount = reader.GetDecimal(reader.GetOrdinal("AllocatedAmount")),
                                AmountUsed = reader.GetDecimal(reader.GetOrdinal("AmountUsed"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateCategory(CategoryDTO dto)
        {
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

        public void CreateCategory(CategoryDTO dto)
        {
            //string sql = $"INSERT INTO Category ([CategoryID],[AccountID],[Name],[AllocatedAmount],[AmountUsed]) VALUES (@categoryId, @accountId, {"name"}, @allocatedAmount)";
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

        public CategoryDTO GetUnassignedCategory(Guid accountId)
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
                            return new CategoryDTO
                            {
                                CategoryID = reader.GetGuid(reader.GetOrdinal("CategoryID")),
                                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("Name")),
                                AllocatedAmount = reader.GetDecimal(reader.GetOrdinal("AllocatedAmount")),
                                AmountUsed = reader.GetDecimal(reader.GetOrdinal("AmountUsed"))
                            };
                        }
                    }
                }
            }

            // If not found, create it
            var newCategory = new CategoryDTO
            {
                CategoryID = Guid.NewGuid(),
                AccountID = accountId,
                CategoryName = "Unassigned",
                AllocatedAmount = 0,
                AmountUsed = 0
            };
            CreateCategory(newCategory);
            return newCategory;
        }
    }
}
