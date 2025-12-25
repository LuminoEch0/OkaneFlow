using DataAccessLayer.DataTransferObjects;
using Service.RepoInterface;
using Service.Models;
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

        public Task<UserModel?> GetByUsernameAsync(string username)
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
                            string role = reader.GetString(reader.GetOrdinal("Role"));
                            var model = new UserModel
                            {
                                UserID = reader.GetGuid(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                IsAdmin = role == "Admin",
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
                            };
                            return Task.FromResult<UserModel?>(model);
                        }
                    }
                }
            }

            return Task.FromResult<UserModel?>(null);
        }

        public Task<UserModel?> GetByIdAsync(Guid id)
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
                            string role = reader.GetString(reader.GetOrdinal("Role"));
                            var model = new UserModel
                            {
                                UserID = reader.GetGuid(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                IsAdmin = role == "Admin",
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
                            };
                            return Task.FromResult<UserModel?>(model);
                        }
                    }
                }
            }

            return Task.FromResult<UserModel?>(null);
        }

        public Task<bool> CreateUserAsync(UserModel user)
        {
            string sql = "INSERT INTO [User] ([Username],[Email],[PasswordHash],[CreationDate],[Role]) VALUES (@username, @email, @passwordHash, @creationDate, @role)";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@creationDate", user.CreationDate);
                    cmd.Parameters.AddWithValue("@role", user.IsAdmin ? "Admin" : "User");

                    var rows = cmd.ExecuteNonQuery();
                    return Task.FromResult(rows > 0);
                }
            }
        }

        public Task<bool> UpdateUserAsync(UserModel user)
        {
            string sql = "UPDATE [User] SET Username = @username, Email = @email, PasswordHash = @passwordHash, Role = @role WHERE UserID = @id";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", user.UserID);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@role", user.IsAdmin ? "Admin" : "User");

                    var rows = cmd.ExecuteNonQuery();
                    return Task.FromResult(rows > 0);
                }
            }
        }

        public Task<bool> DeleteUserAsync(Guid userId)
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

        public Task<List<UserModel>> GetAllUsersAsync()
        {
            string sql = "SELECT [UserID],[Username],[Email],[PasswordHash],[CreationDate],[Role] FROM [User]";
            var users = new List<UserModel>();

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string role = reader.GetString(reader.GetOrdinal("Role"));
                            users.Add(new UserModel
                            {
                                UserID = reader.GetGuid(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                IsAdmin = role == "Admin",
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