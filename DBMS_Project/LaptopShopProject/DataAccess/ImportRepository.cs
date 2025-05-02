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
                    using (var cmd = new SqlCommand("SELECT DISTINCT import_id, supplier_id, supplier_name, import_date, total_amount FROM vw_ImportDetails", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                imports.Add(new Import
                                {
                                    ImportId = reader.GetInt32(0),
                                    SupplierId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                    SupplierName = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    ImportDate = reader.GetDateTime(3),
                                    TotalAmount = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view imports.", ex);
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
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view import details.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving import details: " + ex.Message, ex);
            }
            return details;
        }

        public async Task<int> InsertImportAsync(Import import)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertImport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@supplier_id", import.SupplierId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@import_date", import.ImportDate);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 547 || ex.Message.Contains("Nhà cung cấp không tồn tại"))
            {
                throw new InvalidOperationException("Invalid supplier ID.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to insert imports.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting import: " + ex.Message, ex);
            }
        }

        public async Task<int> InsertImportDetailAsync(ImportDetail detail, decimal? price = null, string categoryName = null, string categoryDescription = null)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertImportDetail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@import_id", detail.ImportId);
                        cmd.Parameters.AddWithValue("@product_name", detail.ProductName);
                        cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                        cmd.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
                        cmd.Parameters.AddWithValue("@price", price ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@category_name", categoryName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@category_description", categoryDescription ?? (object)DBNull.Value);
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
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Message.Contains("Sản phẩm đã tồn tại") || ex.Message.Contains("Thương hiệu đã tồn tại"))
            {
                throw new InvalidOperationException("A product or category with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to insert import details.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting import detail: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateImportAsync(ImportDetail detail, decimal newPrice)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateImport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@import_id", detail.ImportId);
                        cmd.Parameters.AddWithValue("@product_name", detail.ProductName);
                        cmd.Parameters.AddWithValue("@new_quantity", detail.Quantity);
                        cmd.Parameters.AddWithValue("@new_unit_price", detail.UnitPrice);
                        cmd.Parameters.AddWithValue("@new_price", newPrice);

                        var isUpdatedParam = new SqlParameter("@is_updated", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(isUpdatedParam);

                        await cmd.ExecuteNonQueryAsync();

                        return (bool)isUpdatedParam.Value;
                    }
                }
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
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to update import details.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating import: " + ex.Message, ex);
            }
        }

        public async Task DeleteImportAsync(int importId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteImport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@import_id", importId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Phiếu nhập không tồn tại"))
            {
                throw new KeyNotFoundException("Import does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete imports.", ex);
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
                        var result = await cmd.ExecuteScalarAsync();
                        return result == DBNull.Value ? 0m : (decimal)result;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to calculate import total.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error calculating import total: " + ex.Message, ex);
            }
        }
    }
}