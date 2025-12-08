using DataAccessLayer.DataTransferObjects;
using DataAccessLayer.Repositories.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly ConnectionManager _dbManager;

        public UserRepo(ConnectionManager dbManager)
        {   
            _dbManager = dbManager;
        }

        public Task<UserDTO?> GetByUsernameAsync(string username)
        {
            string sql = "SELECT [UserID],[Username],[Email],[PasswordHash],[CreationDate],[Role] FROM [User] WHERE Username = @username";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var dto = new UserDTO
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                HashedPassword = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
                            };
                            return Task.FromResult<UserDTO?>(dto);
                        }
                    }
                }
            }

            return Task.FromResult<UserDTO?>(null);
        }

        public Task<UserDTO?> GetByIdAsync(int id)
        {
            string sql = "SELECT TOP (1) [UserID],[Username],[Email],[PasswordHash],[CreationDate],[Role] FROM [User] WHERE UserID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var dto = new UserDTO
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                HashedPassword = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
                            };
                            return Task.FromResult<UserDTO?>(dto);
                        }
                    }
                }
            }

            return Task.FromResult<UserDTO?>(null);
        }

        public Task<bool> CreateUserAsync(UserDTO user)
        {
            string sql = "INSERT INTO [User] ([Username],[Email],[PasswordHash],[CreationDate],[Role]) VALUES (@username, @email, @passwordHash, @creationDate, @role)";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@username", user.Username ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@passwordHash", user.HashedPassword ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@creationDate", user.CreationDate);
                    cmd.Parameters.AddWithValue("@role", user.Role ?? "User");

                    var rows = cmd.ExecuteNonQuery();
                    return Task.FromResult(rows > 0);
                }
            }
        }

        public Task<bool> UpdateUserAsync(UserDTO user)
        {
            string sql = "UPDATE [User] SET Username = @username, Email = @email, PasswordHash = @passwordHash, Role = @role WHERE UserID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.Parameters.AddWithValue("@username", user.Username ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@passwordHash", user.HashedPassword ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@role", user.Role ?? "User");

                    var rows = cmd.ExecuteNonQuery();
                    return Task.FromResult(rows > 0);
                }
            }
        }

        public Task<bool> DeleteUserAsync(int userId)
        {
            string sql = "DELETE FROM [User] WHERE UserID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    var rows = cmd.ExecuteNonQuery();
                    return Task.FromResult(rows > 0);
                }
            }
        }

        public Task<List<UserDTO>> GetAllUsersAsync()
        {
            string sql = "SELECT [UserID],[Username],[Email],[PasswordHash],[CreationDate],[Role] FROM [User]";
            var users = new List<UserDTO>();

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserDTO
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                HashedPassword = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
                            });
                        }
                    }
                }
            }
            return Task.FromResult(users);
        }
    }
}