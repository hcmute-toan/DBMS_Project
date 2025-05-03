using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    public class SupplierRepository
    {
        private readonly string _username;
        private readonly string _password;

        public SupplierRepository(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private SqlConnection GetConnection()
        {
            string connectionString = $"Server=TonyNyan\\TONYNYAN;Database=LaptopStoreDBMS4;User Id={_username};Password={_password};";
            return new SqlConnection(connectionString);
        }

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            var suppliers = new List<Supplier>();
            try
            {
                using (var conn = GetConnection())
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
                                    ContactInfo = reader.IsDBNull(2) ? null : reader.GetString(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view suppliers.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving suppliers: " + ex.Message, ex);
            }
            return suppliers;
        }

        public async Task<int> InsertSupplierAsync(Supplier supplier)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertSupplier", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@supplier_name", supplier.SupplierName);
                        cmd.Parameters.AddWithValue("@contact_info", supplier.ContactInfo ?? (object)DBNull.Value);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Message.Contains("Nhà cung cấp đã tồn tại"))
            {
                throw new InvalidOperationException("A supplier with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to insert suppliers.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting supplier: " + ex.Message, ex);
            }
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateSupplier", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@supplier_id", supplier.SupplierId);
                        cmd.Parameters.AddWithValue("@supplier_name", supplier.SupplierName);
                        cmd.Parameters.AddWithValue("@contact_info", supplier.ContactInfo ?? (object)DBNull.Value);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Message.Contains("Nhà cung cấp đã tồn tại"))
            {
                throw new InvalidOperationException("A supplier with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Nhà cung cấp không tồn tại"))
            {
                throw new KeyNotFoundException("Supplier does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to update suppliers.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating supplier: " + ex.Message, ex);
            }
        }

        public async Task DeleteSupplierAsync(int supplierId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteSupplier", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Nhà cung cấp không tồn tại"))
            {
                throw new KeyNotFoundException("Supplier does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete suppliers.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("FOREIGN KEY"))
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