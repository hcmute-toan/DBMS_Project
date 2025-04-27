using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class ReportRepository
    {
        public async Task<List<Product>> GetInventoryReportAsync()
        {
            var products = new List<Product>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT * FROM vw_Inventory", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                products.Add(new Product
                                {
                                    ProductId = reader.GetInt32(0), // product_id
                                    ProductName = reader.GetString(1), // product_name
                                    Price = reader.GetDecimal(2), // price
                                    StockQuantity = reader.GetInt32(3), // stock_quantity
                                    Brands = reader.IsDBNull(4) ? null : reader.GetString(4) // brands (correct index)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving inventory report: " + ex.Message, ex);
            }
            return products;
        }

        public async Task<List<Import>> GetImportReportAsync()
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
                throw new Exception("Error retrieving import report: " + ex.Message, ex);
            }
            return imports;
        }

        public async Task<List<Export>> GetExportReportAsync()
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
                throw new Exception("Error retrieving export report: " + ex.Message, ex);
            }
            return exports;
        }
    }
}