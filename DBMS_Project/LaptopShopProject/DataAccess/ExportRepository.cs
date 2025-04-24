using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    internal class ExportRepository
    {

        private readonly string connectionString = "Data Source=YNWA\\SQLEXPRESS;Initial Catalog=DBMS_2;Integrated Security=True";

        // Get all exports (Read)
        public List<Export> GetAllExports()
        {
            List<Export> exports = new List<Export>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM vw_ExportDetails", conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        exports.Add(new Export
                        {
                            ExportId = Convert.ToInt32(reader["export_id"]),
                            ExportDate = Convert.ToDateTime(reader["export_date"]),
                            CustomerName = reader["customer_name"].ToString(),
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            ProductName = reader["product_name"].ToString(),
                            Quantity = Convert.ToInt32(reader["quantity"]),
                            UnitPrice = Convert.ToDecimal(reader["unit_price"]),
                            TotalAmount = Convert.ToDecimal(reader["total_amount"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching exports: {ex.Message}", ex);
            }
            return exports;
        }

        // Insert a new export (Create)
        public int InsertExport(DateTime exportDate, int customerId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertExport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@export_date", exportDate);
                    cmd.Parameters.AddWithValue("@customer_id", customerId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting export: {ex.Message}", ex);
            }
        }

        // Insert export detail
        public void InsertExportDetail(int exportId, int productId, int quantity, decimal unitPrice)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertExportDetail", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@export_id", exportId);
                    cmd.Parameters.AddWithValue("@product_id", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@unit_price", unitPrice);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting export detail: {ex.Message}", ex);
            }
        }

        // Update an export (Update)
        public void UpdateExport(int exportId, DateTime exportDate, int customerId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateExport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@export_id", exportId);
                    cmd.Parameters.AddWithValue("@export_date", exportDate);
                    cmd.Parameters.AddWithValue("@customer_id", customerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating export: {ex.Message}", ex);
            }
        }

        // Delete an export (Delete)
        public void DeleteExport(int exportId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteExport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@export_id", exportId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting export: {ex.Message}", ex);
            }
        }

        // Get all customers for ComboBox
        public DataTable GetCustomers()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllCustomers", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching customers: {ex.Message}", ex);
            }
            return dt;
        }

        // Get all products for ComboBox
        public DataTable GetProducts()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllProducts", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching products: {ex.Message}", ex);
            }
            return dt;
        }
    }
}