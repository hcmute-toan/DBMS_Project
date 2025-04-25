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
    public class SupplierRepository
    {
        public List<Supplier> GetAllSuppliers(int currentUserId)
        {
            var suppliers = new List<Supplier>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_GetAllSuppliers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suppliers.Add(new Supplier
                            {
                                SupplierId = reader.GetInt32(0),
                                SupplierName = reader.GetString(1),
                                ContactInfo = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return suppliers;
        }

        public void InsertSupplier(int currentUserId, Supplier supplier)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertSupplier", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@supplier_name", supplier.SupplierName);
                    cmd.Parameters.AddWithValue("@contact_info", supplier.ContactInfo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateSupplier(int currentUserId, Supplier supplier)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_UpdateSupplier", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@supplier_id", supplier.SupplierId);
                    cmd.Parameters.AddWithValue("@supplier_name", supplier.SupplierName);
                    cmd.Parameters.AddWithValue("@contact_info", supplier.ContactInfo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSupplier(int currentUserId, int supplierId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_DeleteSupplier", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
