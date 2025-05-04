using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    public class UserRepository
    {
        private readonly string _username;
        private readonly string _password;

        public UserRepository(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private SqlConnection GetConnection()
        {
            string connectionString = $"Data Source=TWELVE-T\\TWELVETI;Initial Catalog=LaptopStoreDBMS4;User Id= {_username} ;Password= {_password} ;Integrated Security=True;Trust Server Certificate=True";
            return new SqlConnection(connectionString);
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT user_name, user_type, roles, create_date FROM vw_Users", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    UserId = 0, // View không cung cấp UserId, nên để mặc định là 0
                                    Username = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                                    Role = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                    CreateDate = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3)
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
    }
}