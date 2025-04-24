using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    internal class SupplierRepository
    {
        private readonly string connectionString = "Data Source=YNWA\\SQLEXPRESS;Initial Catalog=DBMS_2;Integrated Security=True";

        // Get all suppliers (Read)
        public List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllSuppliers", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        suppliers.Add(new Supplier
                        {
                            SupplierId = Convert.ToInt32(reader["supplier_id"]),
                            Name = reader["name"].ToString(),
                            Gmail = reader["gmail"].ToString(),
                            Phone = reader["phone"].ToString()
                            // ImportDate is not directly in Supplier table, can be fetched from Import if needed
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching suppliers: {ex.Message}", ex);
            }
            return suppliers;
        }

        // Insert a new supplier (Create)
        public int InsertSupplier(string name, string gmail, string phone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertSupplier", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@gmail", gmail);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting supplier: {ex.Message}", ex);
            }
        }

        // Update a supplier (Update)
        public void UpdateSupplier(int supplierId, string name, string gmail, string phone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateSupplier", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@gmail", gmail);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating supplier: {ex.Message}", ex);
            }
        }

        // Delete a supplier (Delete)
        public void DeleteSupplier(int supplierId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteSupplier", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting supplier: {ex.Message}", ex);
            }
        }
    }

}