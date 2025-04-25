using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class CategoryRepository
    {
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT category_id, category_name, description FROM Category", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryId = reader.GetInt32(0),
                                CategoryName = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return categories;
        }

        public void InsertCategory(int currentUserId, Category category)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@category_name", category.CategoryName);
                    cmd.Parameters.AddWithValue("@description", category.Description);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCategory(int currentUserId, Category category)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_UpdateCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@category_id", category.CategoryId);
                    cmd.Parameters.AddWithValue("@category_name", category.CategoryName);
                    cmd.Parameters.AddWithValue("@description", category.Description);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCategory(int currentUserId, int categoryId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_DeleteCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
