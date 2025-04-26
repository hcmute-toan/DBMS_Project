using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class UserRepository
    {
        public User Login(string username, string password)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Role = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void InsertUser(int currentUserId, string username, string password, string role)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_InsertUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // Ném lại lỗi với thông báo từ cơ sở dữ liệu
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void UpdateUser(int currentUserId, int userId, string username, string password)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_UpdateUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void UpdateUserRole(int currentUserId, int userId, string role)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_UpdateUserRole", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteUser(int currentUserId, int userId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_DeleteUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public List<User> GetAllUsers(int currentUserId)
        {
            var users = new List<User>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_GetAllUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Role = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return users;
        }

        public List<PermissionLog> GetPermissionLogs(int currentUserId)
        {
            var logs = new List<PermissionLog>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_GetPermissionLogs", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    using ( var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new PermissionLog
                            {
                                LogId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Action = reader.GetString(2),
                                OldRole = reader.IsDBNull(3) ? null : reader.GetString(3),
                                NewRole = reader.IsDBNull(4) ? null : reader.GetString(4),
                                ActionDate = reader.GetDateTime(5),
                                PerformedBy = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            return logs;
        }
    }
}
