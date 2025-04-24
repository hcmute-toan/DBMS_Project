using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    internal class ProductRepository
    {
        private readonly string connectionString = "Data Source=YNWA\\SQLEXPRESS;Initial Catalog=DBMS_2;Integrated Security=True";

        // Get all products (Read)
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllProducts", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            ProductName = reader["product_name"].ToString(),
                            Description = reader["description"].ToString(),
                            Brand = reader["brand"].ToString(),
                            Ean13 = reader["ean13"].ToString(),
                            ImportPrice = Convert.ToDecimal(reader["import_price"]),
                            WholesalePrice = Convert.ToDecimal(reader["wholesale_price"]),
                            RetailPrice = Convert.ToDecimal(reader["retail_price"]),
                            ImagePath = reader["image_path"].ToString(),
                            WarehouseId = Convert.ToInt32(reader["warehouse_id"]),
                            Status = Convert.ToBoolean(reader["status"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching products: {ex.Message}", ex);
            }
            return products;
        }

        // Insert a new product (Create)
        public int InsertProduct(string productName, string description, string brand, string ean13, decimal importPrice, decimal wholesalePrice, decimal retailPrice, string imagePath, int warehouseId, bool status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertProduct", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@product_name", productName);
                    cmd.Parameters.AddWithValue("@description", (object)description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@brand", (object)brand ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ean13", (object)ean13 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@import_price", importPrice);
                    cmd.Parameters.AddWithValue("@wholesale_price", wholesalePrice);
                    cmd.Parameters.AddWithValue("@retail_price", retailPrice);
                    cmd.Parameters.AddWithValue("@image_path", (object)imagePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@warehouse_id", warehouseId);
                    cmd.Parameters.AddWithValue("@status", status);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting product: {ex.Message}", ex);
            }
        }

        // Update a product (Update)
        public void UpdateProduct(int productId, string productName, string description, string brand, string ean13, decimal importPrice, decimal wholesalePrice, decimal retailPrice, string imagePath, int warehouseId, bool status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateProduct", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@product_id", productId);
                    cmd.Parameters.AddWithValue("@product_name", productName);
                    cmd.Parameters.AddWithValue("@description", (object)description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@brand", (object)brand ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ean13", (object)ean13 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@import_price", importPrice);
                    cmd.Parameters.AddWithValue("@wholesale_price", wholesalePrice);
                    cmd.Parameters.AddWithValue("@retail_price", retailPrice);
                    cmd.Parameters.AddWithValue("@image_path", (object)imagePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@warehouse_id", warehouseId);
                    cmd.Parameters.AddWithValue("@status", status);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}", ex);
            }
        }

        // Delete a product (Delete)
        public void DeleteProduct(int productId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteProduct", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@product_id", productId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product: {ex.Message}", ex);
            }
        }
    }

}