using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class UserRepository
    {
        public async Task<User> LoginAsync(string username, string password)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_Login", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
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
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to login.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error during login: " + ex.Message, ex);
            }
        }

        public async Task<int> InsertUserAsync(string username, string password, string role)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@role", role);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Message.Contains("Tài khoản đã tồn tại"))
            {
                throw new InvalidOperationException("A user with this username already exists.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to create users.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Vai trò không hợp lệ"))
            {
                throw new InvalidOperationException("Invalid role. Must be 'admin_role' or 'employee_role'.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting user: " + ex.Message, ex);
            }
        }

        public async Task UpdateUserAsync(int userId, string username, string password)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Message.Contains("Tài khoản đã tồn tại"))
            {
                throw new InvalidOperationException("A user with this username already exists.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to update users.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Tài khoản không tồn tại"))
            {
                throw new KeyNotFoundException("User does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating user: " + ex.Message, ex);
            }
        }

        public async Task UpdateUserRoleAsync(int userId, string role)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateUserRole", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@role", role);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to update user roles.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Tài khoản không tồn tại"))
            {
                throw new KeyNotFoundException("User does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Vai trò không hợp lệ"))
            {
                throw new InvalidOperationException("Invalid role. Must be 'admin_role' or 'employee_role'.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating user role: " + ex.Message, ex);
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete users.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Tài khoản không tồn tại"))
            {
                throw new KeyNotFoundException("User does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Không thể xóa chính tài khoản"))
            {
                throw new InvalidOperationException("Cannot delete your own account.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error deleting user: " + ex.Message, ex);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_GetAllUsers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view user list.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving users: " + ex.Message, ex);
            }
            return users;
        }

        public async Task<List<PermissionLog>> GetPermissionLogsAsync()
        {
            var logs = new List<PermissionLog>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_GetPermissionLogs", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                logs.Add(new PermissionLog
                                {
                                    LogId = reader.GetInt32(0),
                                    Action = reader.GetString(1),
                                    ActionDate = reader.GetDateTime(2),
                                    PerformedBy = reader.GetString(3),
                                    TargetRole = reader.IsDBNull(4) ? null : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view permission logs.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving permission logs: " + ex.Message, ex);
            }
            return logs;
        }
    }
}