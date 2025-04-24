using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    internal class ImportRepository
    {
        private readonly string connectionString = "Data Source=YNWA\\SQLEXPRESS;Initial Catalog=DBMS_2;Integrated Security=True";

        // Get all imports (Read)
        public List<Import> GetAllImports()
        {
            List<Import> imports = new List<Import>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "SELECT id.*, i.supplier_id " +
                        "FROM vw_ImportDetails id " +
                        "JOIN Import i ON id.import_id = i.import_id", conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        imports.Add(new Import
                        {
                            ImportId = Convert.ToInt32(reader["import_id"]),
                            ImportDate = Convert.ToDateTime(reader["import_date"]),
                            SupplierId = Convert.ToInt32(reader["supplier_id"]),
                            SupplierName = reader["supplier_name"].ToString(),
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
                throw new Exception($"Error fetching imports: {ex.Message}", ex);
            }
            return imports;
        }

        // Insert a new import (Create)
        public int InsertImport(DateTime importDate, int supplierId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertImport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@import_date", importDate);
                    cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting import: {ex.Message}", ex);
            }
        }

        // Insert import detail
        public void InsertImportDetail(int importId, int productId, int quantity, decimal unitPrice)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertImportDetail", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@import_id", importId);
                    cmd.Parameters.AddWithValue("@product_id", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@unit_price", unitPrice);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting import detail: {ex.Message}", ex);
            }
        }

        // Update an import (Update)
        public void UpdateImport(int importId, DateTime importDate, int supplierId, int productId, int quantity, decimal unitPrice)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Update the Import table
                    SqlCommand cmd = new SqlCommand("sp_UpdateImport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@import_id", importId);
                    cmd.Parameters.AddWithValue("@import_date", importDate);
                    cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                    cmd.ExecuteNonQuery();

                    // Delete existing ImportDetail records for this import
                    SqlCommand deleteCmd = new SqlCommand("DELETE FROM ImportDetail WHERE import_id = @import_id", conn);
                    deleteCmd.Parameters.AddWithValue("@import_id", importId);
                    deleteCmd.ExecuteNonQuery();

                    // Insert new ImportDetail record
                    InsertImportDetail(importId, productId, quantity, unitPrice);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating import: {ex.Message}", ex);
            }
        }

        // Delete an import (Delete)
        public void DeleteImport(int importId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteImport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@import_id", importId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting import: {ex.Message}", ex);
            }
        }

        // Get all suppliers for ComboBox
        public DataTable GetSuppliers()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllSuppliers", conn)
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
                throw new Exception($"Error fetching suppliers: {ex.Message}", ex);
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