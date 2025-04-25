using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace LaptopShopProject.DataAccess
{
    public class CategoryRepository
    {
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            try
            {
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
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving categories from the database: " + ex.Message, ex);
            }
            return categories;
        }

        public void InsertCategory(int currentUserId, Category category)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_InsertCategory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@category_name", category.CategoryName);
                        cmd.Parameters.AddWithValue("@description", category.Description ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A category with this name already exists.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting category: " + ex.Message, ex);
            }
        }

        public void UpdateCategory(int currentUserId, Category category)
        {
            try
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
                        cmd.Parameters.AddWithValue("@description", category.Description ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A category with this name already exists.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating category: " + ex.Message, ex);
            }
        }

        public void DeleteCategory(int currentUserId, int categoryId)
        {
            try
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
            catch (SqlException ex)
            {
                throw new Exception("Error deleting category: " + ex.Message, ex);
            }
        }
    }
}