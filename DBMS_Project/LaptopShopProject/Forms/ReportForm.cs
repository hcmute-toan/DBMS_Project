using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml; // For EPPlus
using System.IO;      // For File operations
using System.Linq;    // For LINQ
using System.Drawing; // For Color (used in Designer)

namespace LaptopShopProject.Forms
{
    public partial class ReportForm : UserControl
    {
        private readonly ReportRepository _reportRepository;
        private readonly User _currentUser;

        public ReportForm(User currentUser)
        {
            InitializeComponent();
            // Set EPPlus License for EPPlus 5 (non-commercial use)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _reportRepository = new ReportRepository();
            _currentUser = currentUser;
            LoadReportsAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            btnRefresh.Click += btnRefresh_Click;
            btnExportToExcel.Click += btnExportToExcel_Click;
        }

        private async Task LoadReportsAsync()
        {
            try
            {
                var inventory = await _reportRepository.GetInventoryReportAsync();
                var imports = await _reportRepository.GetImportReportAsync();
                var exports = await _reportRepository.GetExportReportAsync();

                // Clear existing controls in case of refresh
                dgvInventory.Controls.Clear();
                dgvImportReport.Controls.Clear();
                dgvExportReport.Controls.Clear();

                // Create and add new DataGridViews
                var dgvInventoryGrid = new DataGridView { Dock = DockStyle.Fill, Name = "dgvInventoryGrid" };
                var dgvImportReportGrid = new DataGridView { Dock = DockStyle.Fill, Name = "dgvImportReportGrid" };
                var dgvExportReportGrid = new DataGridView { Dock = DockStyle.Fill, Name = "dgvExportReportGrid" };

                dgvInventory.Controls.Add(dgvInventoryGrid);
                dgvImportReport.Controls.Add(dgvImportReportGrid);
                dgvExportReport.Controls.Add(dgvExportReportGrid);

                dgvInventoryGrid.DataSource = inventory;
                dgvImportReportGrid.DataSource = imports;
                dgvExportReportGrid.DataSource = exports;

                ConfigureInventoryGrid(dgvInventoryGrid);
                ConfigureImportReportGrid(dgvImportReportGrid);
                ConfigureExportReportGrid(dgvExportReportGrid);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureInventoryGrid(DataGridView dgv)
        {
            // Basic ReadOnly and SelectionMode settings
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            // Column configuration
            if (dgv.Columns.Contains("ProductId"))
                dgv.Columns["ProductId"].HeaderText = "ID";
            if (dgv.Columns.Contains("ProductName"))
                dgv.Columns["ProductName"].HeaderText = "Name";
            if (dgv.Columns.Contains("Price"))
            {
                dgv.Columns["Price"].HeaderText = "Price";
                dgv.Columns["Price"].DefaultCellStyle.Format = "N0"; // Format for Vietnamese Dong (e.g., 1,234,567)
            }
            if (dgv.Columns.Contains("StockQuantity"))
                dgv.Columns["StockQuantity"].HeaderText = "Stock Quantity";
            if (dgv.Columns.Contains("Brands"))
                dgv.Columns["Brands"].HeaderText = "Brands";

            // Hide properties from base Model class if they exist and aren't needed
            if (dgv.Columns.Contains("Error")) dgv.Columns["Error"].Visible = false;
            if (dgv.Columns.Contains("HasError")) dgv.Columns["HasError"].Visible = false;
            if (dgv.Columns.Contains("ValidationErrors")) dgv.Columns["ValidationErrors"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureImportReportGrid(DataGridView dgv)
        {
            // Basic ReadOnly and SelectionMode settings
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            // Column configuration
            if (dgv.Columns.Contains("ImportId"))
                dgv.Columns["ImportId"].HeaderText = "ID";
            if (dgv.Columns.Contains("SupplierId"))
                dgv.Columns["SupplierId"].Visible = false;
            if (dgv.Columns.Contains("SupplierName"))
                dgv.Columns["SupplierName"].HeaderText = "Supplier";
            if (dgv.Columns.Contains("ImportDate"))
            {
                dgv.Columns["ImportDate"].HeaderText = "Date";
                dgv.Columns["ImportDate"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }
            if (dgv.Columns.Contains("TotalAmount"))
            {
                dgv.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgv.Columns["TotalAmount"].DefaultCellStyle.Format = "N0"; // Format for Vietnamese Dong
            }

            // Hide properties from base Model class if they exist and aren't needed
            if (dgv.Columns.Contains("Error")) dgv.Columns["Error"].Visible = false;
            if (dgv.Columns.Contains("HasError")) dgv.Columns["HasError"].Visible = false;
            if (dgv.Columns.Contains("ValidationErrors")) dgv.Columns["ValidationErrors"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureExportReportGrid(DataGridView dgv)
        {
            // Basic ReadOnly and SelectionMode settings
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            // Column configuration
            if (dgv.Columns.Contains("ExportId"))
                dgv.Columns["ExportId"].HeaderText = "ID";
            if (dgv.Columns.Contains("CustomerId"))
                dgv.Columns["CustomerId"].Visible = false;
            if (dgv.Columns.Contains("CustomerName"))
                dgv.Columns["CustomerName"].HeaderText = "Customer";
            if (dgv.Columns.Contains("ExportDate"))
            {
                dgv.Columns["ExportDate"].HeaderText = "Date";
                dgv.Columns["ExportDate"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }
            if (dgv.Columns.Contains("TotalAmount"))
            {
                dgv.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgv.Columns["TotalAmount"].DefaultCellStyle.Format = "N0"; // Format for Vietnamese Dong
            }

            // Hide properties from base Model class if they exist and aren't needed
            if (dgv.Columns.Contains("Error")) dgv.Columns["Error"].Visible = false;
            if (dgv.Columns.Contains("HasError")) dgv.Columns["HasError"].Visible = false;
            if (dgv.Columns.Contains("ValidationErrors")) dgv.Columns["ValidationErrors"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadReportsAsync();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            // 1. Identify the currently selected TabPage
            TabPage selectedTabPage = tabControl1.SelectedTab;
            if (selectedTabPage == null)
            {
                MessageBox.Show("No report tab selected.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Find the DataGridView within the selected TabPage
            DataGridView dgvToExport = selectedTabPage.Controls.OfType<DataGridView>().FirstOrDefault();
            if (dgvToExport == null)
            {
                MessageBox.Show("Could not find the data grid on the selected tab. Please ensure data is loaded.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dgvToExport.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to export.", "Export Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 3. Prompt user for save location
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                string safeTabText = selectedTabPage.Text.Replace("Tab ", "");
                foreach (char invalidChar in Path.GetInvalidFileNameChars())
                {
                    safeTabText = safeTabText.Replace(invalidChar, '_');
                }
                string suggestedName = $"{safeTabText}_Report_{DateTime.Now:yyyyMMdd}.xlsx";
                saveFileDialog.FileName = suggestedName;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileInfo excelFile = new FileInfo(saveFileDialog.FileName);

                        // 4. Use EPPlus to create and save the Excel file
                        using (ExcelPackage package = new ExcelPackage(excelFile))
                        {
                            string worksheetName = safeTabText.Length > 31 ? safeTabText.Substring(0, 31) : safeTabText;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

                            // 5. Write Headers (from visible columns)
                            int colIndex = 1;
                            foreach (DataGridViewColumn column in dgvToExport.Columns)
                            {
                                if (column.Visible)
                                {
                                    worksheet.Cells[1, colIndex].Value = column.HeaderText;
                                    worksheet.Cells[1, colIndex].Style.Font.Bold = true;
                                    colIndex++;
                                }
                            }

                            // 6. Write Data Rows
                            int rowIndex = 2;
                            foreach (DataGridViewRow row in dgvToExport.Rows)
                            {
                                if (row.IsNewRow) continue;

                                colIndex = 1;
                                foreach (DataGridViewColumn column in dgvToExport.Columns)
                                {
                                    if (column.Visible)
                                    {
                                        object formattedValue = row.Cells[column.Index].FormattedValue;
                                        worksheet.Cells[rowIndex, colIndex].Value = formattedValue ?? string.Empty;

                                        object actualValue = row.Cells[column.Index].Value;
                                        if (actualValue is decimal || actualValue is double || actualValue is float)
                                        {
                                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0"; // Vietnamese Dong format
                                        }
                                        else if (actualValue is int || actualValue is long)
                                        {
                                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
                                        }
                                        else if (actualValue is DateTime dt)
                                        {
                                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                                            worksheet.Cells[rowIndex, colIndex].Value = dt;
                                        }

                                        colIndex++;
                                    }
                                }
                                rowIndex++;
                            }

                            // 7. Auto-fit columns for better readability
                            if (worksheet.Dimension != null)
                            {
                                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            }

                            // 8. Add formatting: borders and header background
                            if (worksheet.Dimension != null)
                            {
                                var tableRange = worksheet.Cells[1, 1, rowIndex - 1, colIndex - 1];
                                tableRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                tableRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                tableRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                tableRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                                worksheet.Cells[1, 1, 1, colIndex - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                worksheet.Cells[1, 1, 1, colIndex - 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            }

                            // 9. Save the package
                            package.Save();
                        }

                        MessageBox.Show($"Report successfully exported to:\n{saveFileDialog.FileName}", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show($"Error saving file (it might be open or you lack permissions):\n{ioEx.Message}", "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting data to Excel: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}