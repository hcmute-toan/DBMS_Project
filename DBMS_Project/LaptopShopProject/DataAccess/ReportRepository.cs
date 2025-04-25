using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class ReportRepository
    {
        public List<Product> GetInventoryReport()
        {
            var products = new List<Product>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM vw_Inventory", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                StockQuantity = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            return products;
        }

        public List<Import> GetImportReport()
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

        public List<Export> GetExportReport()
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
    }
}
