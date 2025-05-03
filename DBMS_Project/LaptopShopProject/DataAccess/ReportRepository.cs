using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    public class ReportRepository
    {
        private readonly string _username;
        private readonly string _password;

        public ReportRepository(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private SqlConnection GetConnection()
        {
            string connectionString = $"Data Source=TWELVE-T\\TWELVETI;Initial Catalog=LaptopStoreDBMS4;User Id={_username};Password={_password};Integrated Security=True;Trust Server Certificate=True";
            return new SqlConnection(connectionString);
        }

        public async Task<List<Product>> GetInventoryReportAsync()
        {
            var products = new List<Product>();
            try
            {
                using (var conn = GetConnection())
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
                                    ProductId = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    StockQuantity = reader.GetInt32(3),
                                    Brands = reader.IsDBNull(4) ? null : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view inventory report.", ex);
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
                using (var conn = GetConnection())
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
                throw new UnauthorizedAccessException("You do not have permission to view import report.", ex);
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
                using (var conn = GetConnection())
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
                                    CustomerId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                    CustomerName = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    ExportDate = reader.GetDateTime(3),
                                    TotalAmount = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view export report.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving export report: " + ex.Message, ex);
            }
            return exports;
        }

        public async Task<List<RevenueReport>> GetRevenueByMonthAsync(int year)
        {
            var revenueReports = new List<RevenueReport>();
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_GetRevenueByMonth", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@year", year);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                revenueReports.Add(new RevenueReport
                                {
                                    Month = reader.GetInt32(0),
                                    TotalRevenue = reader.GetDecimal(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view revenue by month report.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving revenue by month report: " + ex.Message, ex);
            }
            return revenueReports;
        }

        public async Task<List<RevenueReport>> GetRevenueByDayAsync(DateTime date)
        {
            var revenueReports = new List<RevenueReport>();
            try
            {
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_GetRevenueByDay", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@date", date);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                revenueReports.Add(new RevenueReport
                                {
                                    Date = reader.GetDateTime(0),
                                    TotalRevenue = reader.GetDecimal(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                throw new UnauthorizedAccessException("You do not have permission to view revenue by day report.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving revenue by day report: " + ex.Message, ex);
            }
            return revenueReports;
        }
    }

    public class RevenueReport
    {
        public int? Month { get; set; }
        public DateTime? Date { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}