using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class SupplierRepository
    {
        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            var suppliers = new List<Supplier>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT supplier_id, supplier_name, contact_info FROM vw_Suppliers", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving suppliers: " + ex.Message, ex);
            }
            return suppliers;
        }

        public async Task<int> InsertSupplierAsync(int currentUserId, Supplier supplier)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertSupplier", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@supplier_name", supplier.SupplierName);
                        cmd.Parameters.AddWithValue("@contact_info", supplier.ContactInfo);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A supplier with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can insert suppliers.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting supplier: " + ex.Message, ex);
            }
        }

        public async Task UpdateSupplierAsync(int currentUserId, Supplier supplier)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateSupplier", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@supplier_id", supplier.SupplierId);
                        cmd.Parameters.AddWithValue("@supplier_name", supplier.SupplierName);
                        cmd.Parameters.AddWithValue("@contact_info", supplier.ContactInfo);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A supplier with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can update suppliers.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Nhà cung cấp không tồn tại"))
            {
                throw new KeyNotFoundException("Supplier does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating supplier: " + ex.Message, ex);
            }
        }

        public async Task DeleteSupplierAsync(int currentUserId, int supplierId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteSupplier", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can delete suppliers.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Nhà cung cấp không tồn tại"))
            {
                throw new KeyNotFoundException("Supplier does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("phiếu nhập liên quan"))
            {
                throw new InvalidOperationException("Cannot delete supplier due to related import records.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error deleting supplier: " + ex.Message, ex);
            }
        }
    }
}