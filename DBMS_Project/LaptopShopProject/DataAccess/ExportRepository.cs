using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class ExportRepository
    {
        public async Task<List<Export>> GetAllExportsAsync()
        {
            var exports = new List<Export>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT DISTINCT export_id, customer_id, customer_name, export_date, total_amount FROM vw_ExportDetails", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                exports.Add(new Export
                                {
                                    ExportId = reader.GetInt32(0),
                                    CustomerId = reader.GetInt32(1),
                                    CustomerName = reader.GetString(2),
                                    ExportDate = reader.GetDateTime(3),
                                    TotalAmount = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving exports: " + ex.Message, ex);
            }
            return exports;
        }

        public async Task<List<ExportDetail>> GetExportDetailsAsync(int exportId)
        {
            var details = new List<ExportDetail>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT export_id, product_id, product_name, quantity, unit_price FROM vw_ExportDetails WHERE export_id = @export_id", conn))
                    {
                        cmd.Parameters.AddWithValue("@export_id", exportId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                details.Add(new ExportDetail
                                {
                                    ExportId = reader.GetInt32(0),
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
                throw new Exception("Error retrieving export details: " + ex.Message, ex);
            }
            return details;
        }

        public async Task<int> InsertExportAsync(int currentUserId, Export export)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertExport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@customer_id", export.CustomerId);
                        cmd.Parameters.AddWithValue("@export_date", export.ExportDate);
                        cmd.Parameters.AddWithValue("@total_amount", export.TotalAmount);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException("Invalid customer ID.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting export: " + ex.Message, ex);
            }
        }

        public async Task InsertExportDetailAsync(int currentUserId, ExportDetail detail)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertExportDetail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@export_id", detail.ExportId);
                        cmd.Parameters.AddWithValue("@product_name", detail.ProductName); // Pass ProductName instead of ProductId
                        cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                        cmd.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Số lượng xuất vượt quá tồn kho"))
            {
                throw new InvalidOperationException("Export quantity exceeds stock.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Phiếu xuất không tồn tại"))
            {
                throw new KeyNotFoundException("Export does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Sản phẩm không tồn tại"))
            {
                throw new KeyNotFoundException("Product does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting export detail: " + ex.Message, ex);
            }
        }

        public async Task UpdateExportAsync(int currentUserId, Export export)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateExport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@export_id", export.ExportId);
                        cmd.Parameters.AddWithValue("@customer_id", export.CustomerId);
                        cmd.Parameters.AddWithValue("@export_date", export.ExportDate);
                        cmd.Parameters.AddWithValue("@total_amount", export.TotalAmount);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can update exports.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Phiếu xuất không tồn tại"))
            {
                throw new KeyNotFoundException("Export does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException("Invalid customer ID.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating export: " + ex.Message, ex);
            }
        }

        public async Task DeleteExportAsync(int currentUserId, int exportId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteExport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@export_id", exportId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can delete exports.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Phiếu xuất không tồn tại"))
            {
                throw new KeyNotFoundException("Export does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error deleting export: " + ex.Message, ex);
            }
        }

        public async Task<decimal> GetExportTotalAsync(int exportId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT dbo.fn_GetExportTotal(@export_id)", conn))
                    {
                        cmd.Parameters.AddWithValue("@export_id", exportId);
                        return (decimal)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error calculating export total: " + ex.Message, ex);
            }
        }
    }
}