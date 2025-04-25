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
    public class ProductRepository
    {
        public List<Product> GetAllProducts(int currentUserId)
        {
            var products = new List<Product>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM vw_ProductDetails", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                StockQuantity = reader.GetInt32(3),
                                Brands = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return products;
        }

        public void InsertProduct(int currentUserId, Product product)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@product_name", product.ProductName);
                    cmd.Parameters.AddWithValue("@price", product.Price);
                    cmd.Parameters.AddWithValue("@stock_quantity", product.StockQuantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(int currentUserId, Product product)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_UpdateProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@product_id", product.ProductId);
                    cmd.Parameters.AddWithValue("@product_name", product.ProductName);
                    cmd.Parameters.AddWithValue("@price", product.Price);
                    cmd.Parameters.AddWithValue("@stock_quantity", product.StockQuantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int currentUserId, int productId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_DeleteProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@product_id", productId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetStockQuantity(int productId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT dbo.fn_GetStockQuantity(@product_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@product_id", productId);
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public List<ProductLog> GetProductLogs()
        {
            var logs = new List<ProductLog>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM ProductLog", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new ProductLog
                            {
                                LogId = reader.GetInt32(0),
                                ProductId = reader.GetInt32(1),
                                ProductName = reader.GetString(2),
                                DeletedDate = reader.GetDateTime(3),
                                DeletedBy = reader.IsDBNull(4) ? null : reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return logs;
        }
    }
}
