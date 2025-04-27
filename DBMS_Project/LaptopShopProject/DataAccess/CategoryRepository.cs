using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class CategoryRepository
    {
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = new List<Category>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT category_id, category_name, description FROM Category", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                categories.Add(new Category
                                {
                                    CategoryId = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
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

        public async Task<int> InsertCategoryAsync(int currentUserId, Category category)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertCategory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@category_name", category.CategoryName);
                        cmd.Parameters.AddWithValue("@description", category.Description ?? (object)DBNull.Value);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A category with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Người dùng không tồn tại"))
            {
                throw new UnauthorizedAccessException("User does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting category: " + ex.Message, ex);
            }
        }

        public async Task UpdateCategoryAsync(int currentUserId, Category category)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateCategory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@category_id", category.CategoryId);
                        cmd.Parameters.AddWithValue("@category_name", category.CategoryName);
                        cmd.Parameters.AddWithValue("@description", category.Description ?? (object)DBNull.Value);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A category with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can update categories.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Thương hiệu không tồn tại"))
            {
                throw new KeyNotFoundException("Category does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating category: " + ex.Message, ex);
            }
        }

        public async Task DeleteCategoryAsync(int currentUserId, int categoryId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteCategory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@category_id", categoryId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can delete categories.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Thương hiệu không tồn tại"))
            {
                throw new KeyNotFoundException("Category does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error deleting category: " + ex.Message, ex);
            }
        }
    }
}