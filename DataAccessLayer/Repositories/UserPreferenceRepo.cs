using Service.RepoInterface;
using Service.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class UserPreferenceRepo : IUserPreferenceRepo
    {
        private readonly ConnectionManager _dbManager;

        public UserPreferenceRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public Task<UserPreferenceModel?> GetByUserIdAsync(Guid userId)
        {
            string sql = @"SELECT [PreferenceID], [UserID], [DarkMode], [EmailNotifications], [Currency], [DateFormat] 
                           FROM [UserPreference] WHERE [UserID] = @userId";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var model = new UserPreferenceModel
                            {
                                PreferenceID = reader.GetGuid(reader.GetOrdinal("PreferenceID")),
                                UserID = reader.GetGuid(reader.GetOrdinal("UserID")),
                                DarkMode = reader.GetBoolean(reader.GetOrdinal("DarkMode")),
                                EmailNotifications = reader.GetBoolean(reader.GetOrdinal("EmailNotifications")),
                                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                                DateFormat = reader.GetString(reader.GetOrdinal("DateFormat"))
                            };
                            return Task.FromResult<UserPreferenceModel?>(model);
                        }
                    }
                }
            }

            return Task.FromResult<UserPreferenceModel?>(null);
        }

        public Task<bool> CreateAsync(UserPreferenceModel preference)
        {
            string sql = @"INSERT INTO [UserPreference] ([PreferenceID], [UserID], [DarkMode], [EmailNotifications], [Currency], [DateFormat])
                           VALUES (@preferenceId, @userId, @darkMode, @emailNotifications, @currency, @dateFormat)";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@preferenceId", preference.PreferenceID);
                    cmd.Parameters.AddWithValue("@userId", preference.UserID);
                    cmd.Parameters.AddWithValue("@darkMode", preference.DarkMode);
                    cmd.Parameters.AddWithValue("@emailNotifications", preference.EmailNotifications);
                    cmd.Parameters.AddWithValue("@currency", preference.Currency);
                    cmd.Parameters.AddWithValue("@dateFormat", preference.DateFormat);

                    var rows = cmd.ExecuteNonQuery();
                    return Task.FromResult(rows > 0);
                }
            }
        }

        public Task<bool> UpdateAsync(UserPreferenceModel preference)
        {
            string sql = @"UPDATE [UserPreference] 
                           SET [DarkMode] = @darkMode, 
                               [EmailNotifications] = @emailNotifications, 
                               [Currency] = @currency, 
                               [DateFormat] = @dateFormat 
                           WHERE [PreferenceID] = @preferenceId";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@preferenceId", preference.PreferenceID);
                    cmd.Parameters.AddWithValue("@darkMode", preference.DarkMode);
                    cmd.Parameters.AddWithValue("@emailNotifications", preference.EmailNotifications);
                    cmd.Parameters.AddWithValue("@currency", preference.Currency);
                    cmd.Parameters.AddWithValue("@dateFormat", preference.DateFormat);

                    var rows = cmd.ExecuteNonQuery();
                    return Task.FromResult(rows > 0);
                }
            }
        }
    }
}
