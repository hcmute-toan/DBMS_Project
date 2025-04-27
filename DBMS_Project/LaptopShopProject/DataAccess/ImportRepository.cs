using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class ImportRepository
    {
        public async Task<List<Import>> GetAllImportsAsync()
        {
            var imports = new List<Import>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(
                        "SELECT i.import_id, i.supplier_id, s.supplier_name, i.import_date, i.total_amount " +
                        "FROM Import i " +
                        "JOIN Supplier s ON i.supplier_id = s.supplier_id", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                imports.Add(new Import
                                {
                                    ImportId = reader.GetInt32(0),
                                    SupplierId = reader.GetInt32(1),
                                    SupplierName = reader.GetString(2),
                                    ImportDate = reader.GetDateTime(3),
                                    TotalAmount = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving imports: " + ex.Message, ex);
            }
            return imports;
        }

        public async Task<List<ImportDetail>> GetImportDetailsAsync(int importId)
        {
            var details = new List<ImportDetail>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT import_id, product_id, product_name, quantity, unit_price FROM vw_ImportDetails WHERE import_id = @import_id", conn))
                    {
                        cmd.Parameters.AddWithValue("@import_id", importId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                details.Add(new ImportDetail
                                {
                                    ImportId = reader.GetInt32(0),
                                    ProductId = reader.GetInt32(1),
                                    ProductName = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    UnitPrice = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving import details: " + ex.Message, ex);
            }
            return details;
        }

        public async Task<int> InsertImportAsync(int currentUserId, Import import)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertImport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@supplier_id", import.SupplierId);
                        cmd.Parameters.AddWithValue("@import_date", import.ImportDate);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException("Invalid supplier ID.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting import: " + ex.Message, ex);
            }
        }

        public async Task<int> InsertImportDetailAsync(int currentUserId, ImportDetail detail, decimal? price = null, string categoryName = null, string categoryDescription = null)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertImportDetail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@import_id", detail.ImportId);
                        cmd.Parameters.AddWithValue("@product_name", detail.ProductName);
                        cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                        cmd.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
                        cmd.Parameters.AddWithValue("@price", (object)price ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@category_name", (object)categoryName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@category_description", (object)categoryDescription ?? DBNull.Value);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Phiếu nhập không tồn tại"))
            {
                throw new KeyNotFoundException("Import does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Số lượng phải lớn hơn 0"))
            {
                throw new InvalidOperationException("Quantity must be greater than 0.", ex);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A product or category with this name already exists.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting import detail: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateImportAsync(int currentUserId, ImportDetail detail, decimal newPrice)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateImport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@import_id", detail.ImportId);
                        cmd.Parameters.AddWithValue("@product_name", detail.ProductName);
                        cmd.Parameters.AddWithValue("@new_quantity", detail.Quantity);
                        cmd.Parameters.AddWithValue("@new_unit_price", detail.UnitPrice);
                        cmd.Parameters.AddWithValue("@new_price", newPrice);

                        // Thêm tham số đầu ra
                        var isUpdatedParam = new SqlParameter("@is_updated", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(isUpdatedParam);

                        await cmd.ExecuteNonQueryAsync();

                        // Trả về giá trị của tham số đầu ra
                        return (bool)isUpdatedParam.Value;
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can update import details and product price.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Phiếu nhập không tồn tại"))
            {
                throw new KeyNotFoundException("Import does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Sản phẩm không tồn tại"))
            {
                throw new KeyNotFoundException("Product does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Chi tiết phiếu nhập không tồn tại"))
            {
                throw new KeyNotFoundException("Import detail does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Số lượng phải lớn hơn 0"))
            {
                throw new InvalidOperationException("Quantity must be greater than 0.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating import: " + ex.Message, ex);
            }
        }

        public async Task DeleteImportAsync(int currentUserId, int importId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteImport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@import_id", importId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can delete imports.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Phiếu nhập không tồn tại"))
            {
                throw new KeyNotFoundException("Import does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error deleting import: " + ex.Message, ex);
            }
        }

        public async Task<decimal> GetImportTotalAsync(int importId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT dbo.fn_GetImportTotal(@import_id)", conn))
                    {
                        cmd.Parameters.AddWithValue("@import_id", importId);
                        return (decimal)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error calculating import total: " + ex.Message, ex);
            }
        }
    }
}