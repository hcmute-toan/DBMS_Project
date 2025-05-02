using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class ProductRepository
    {
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT * FROM vw_ProductDetails", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                products.Add(new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    StockQuantity = reader.GetInt32(3),
                                    Brands = reader.IsDBNull(4) ? null : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view products.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving products: " + ex.Message, ex);
            }
            return products;
        }

        public async Task<bool> ProductExistsAsync(string productName)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Product WHERE product_name = @product_name", conn))
                    {
                        cmd.Parameters.AddWithValue("@product_name", productName);
                        int count = (int)await cmd.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to check product existence.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error checking product existence: " + ex.Message, ex);
            }
        }

        public async Task<int> InsertProductAsync(Product product)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@product_name", product.ProductName);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@stock_quantity", product.StockQuantity);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Message.Contains("Sản phẩm đã tồn tại"))
            {
                throw new InvalidOperationException("A product with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to insert products.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting product: " + ex.Message, ex);
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@product_id", product.ProductId);
                        cmd.Parameters.AddWithValue("@product_name", product.ProductName);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@stock_quantity", product.StockQuantity);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Message.Contains("Sản phẩm đã tồn tại"))
            {
                throw new InvalidOperationException("A product with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Sản phẩm không tồn tại"))
            {
                throw new KeyNotFoundException("Product does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to update products.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating product: " + ex.Message, ex);
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@product_id", productId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Sản phẩm không tồn tại"))
            {
                throw new KeyNotFoundException("Product does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete products.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error deleting product: " + ex.Message, ex);
            }
        }

        public async Task<int> GetStockQuantityAsync(int productId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT dbo.fn_GetStockQuantity(@product_id)", conn))
                    {
                        cmd.Parameters.AddWithValue("@product_id", productId);
                        var result = await cmd.ExecuteScalarAsync();
                        return result == DBNull.Value ? 0 : (int)result;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view stock quantity.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving stock quantity: " + ex.Message, ex);
            }
        }

        public async Task<List<ProductLog>> GetProductLogsAsync()
        {
            var logs = new List<ProductLog>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT * FROM ProductLog", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                logs.Add(new ProductLog
                                {
                                    LogId = reader.GetInt32(0),
                                    ProductId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                    ProductName = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    StockQuantity = reader.GetInt32(4),
                                    DeletedDate = reader.GetDateTime(5),
                                    DeletedBy = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view product logs.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving product logs: " + ex.Message, ex);
            }
            return logs;
        }
    }
}