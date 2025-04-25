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
    public class ImportRepository
    {
        public List<Import> GetAllImports()
        {
            var imports = new List<Import>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DISTINCT import_id, supplier_id, supplier_name, import_date, total_amount FROM vw_ImportDetails", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
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
            return imports;
        }

        public List<ImportDetail> GetImportDetails(int importId)
        {
            var details = new List<ImportDetail>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT import_id, product_id, product_name, quantity, unit_price FROM vw_ImportDetails WHERE import_id = @import_id", conn))
                {
                    cmd.Parameters.AddWithValue("@import_id", importId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
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
            return details;
        }

        public int InsertImport(int currentUserId, Import import)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertImport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@supplier_id", import.SupplierId);
                    cmd.Parameters.AddWithValue("@import_date", import.ImportDate);
                    cmd.Parameters.AddWithValue("@total_amount", import.TotalAmount);
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public void InsertImportDetail(int currentUserId, ImportDetail detail, decimal? price = null)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertImportDetail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@import_id", detail.ImportId);
                    cmd.Parameters.AddWithValue("@product_name", detail.ProductName);
                    cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                    cmd.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
                    cmd.Parameters.AddWithValue("@price", (object)price ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateImport(int currentUserId, Import import)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_UpdateImport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@import_id", import.ImportId);
                    cmd.Parameters.AddWithValue("@supplier_id", import.SupplierId);
                    cmd.Parameters.AddWithValue("@import_date", import.ImportDate);
                    cmd.Parameters.AddWithValue("@total_amount", import.TotalAmount);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteImport(int currentUserId, int importId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_DeleteImport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@import_id", importId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetImportTotal(int importId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT dbo.fn_GetImportTotal(@import_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@import_id", importId);
                    return (decimal)cmd.ExecuteScalar();
                }
            }
        }
    }
}
