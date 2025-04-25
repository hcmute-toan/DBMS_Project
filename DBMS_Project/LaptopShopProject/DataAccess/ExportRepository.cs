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
    public class ExportRepository
    {
        public List<Export> GetAllExports()
        {
            var exports = new List<Export>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DISTINCT export_id, customer_id, customer_name, export_date, total_amount FROM vw_ExportDetails", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
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
            return exports;
        }

        public List<ExportDetail> GetExportDetails(int exportId)
        {
            var details = new List<ExportDetail>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT export_id, product_id, product_name, quantity, unit_price FROM vw_ExportDetails WHERE export_id = @export_id", conn))
                {
                    cmd.Parameters.AddWithValue("@export_id", exportId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
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
            return details;
        }

        public int InsertExport(int currentUserId, Export export)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertExport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@customer_id", export.CustomerId);
                    cmd.Parameters.AddWithValue("@export_date", export.ExportDate);
                    cmd.Parameters.AddWithValue("@total_amount", export.TotalAmount);
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public void InsertExportDetail(int currentUserId, ExportDetail detail)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertExportDetail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@export_id", detail.ExportId);
                    cmd.Parameters.AddWithValue("@product_id", detail.ProductId);
                    cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                    cmd.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateExport(int currentUserId, Export export)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_UpdateExport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@export_id", export.ExportId);
                    cmd.Parameters.AddWithValue("@customer_id", export.CustomerId);
                    cmd.Parameters.AddWithValue("@export_date", export.ExportDate);
                    cmd.Parameters.AddWithValue("@total_amount", export.TotalAmount);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteExport(int currentUserId, int exportId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_DeleteExport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@export_id", exportId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetExportTotal(int exportId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT dbo.fn_GetExportTotal(@export_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@export_id", exportId);
                    return (decimal)cmd.ExecuteScalar();
                }
            }
        }
    }
}
