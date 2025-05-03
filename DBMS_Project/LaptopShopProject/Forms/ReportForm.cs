using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LaptopShopProject.Forms
{
    public partial class ReportForm : UserControl
    {
        private readonly ReportRepository _reportRepository;
        private readonly string _username;
        private readonly string _role;
        private readonly string _password;

        public ReportForm(string username, string role, string password)
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _username = username;
            _role = role;
            _password = password;
            _reportRepository = new ReportRepository(_username, _password);
            ConfigurePermissions();
            InitializeRevenueControls();
            LoadReportsAsync();
            ConfigureEventHandlers();
        }

        private void ConfigurePermissions()
        {
            if (_role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
            {
                foreach (TabPage tab in tabControl1.TabPages)
                {
                    if (tab.Name == "tabRevenue")
                    {
                        tab.Enabled = false;
                        break;
                    }
                }
            }
        }

        private void InitializeRevenueControls()
        {
            foreach (TabPage tab in tabControl1.TabPages)
            {
                if (tab.Name == "tabRevenue")
                {
                    tab.Controls.Clear();

                    // Revenue Type ComboBox
                    var cbRevenueType = new ComboBox
                    {
                        Location = new Point(10, 10),
                        Width = 150,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cbRevenueType.Items.AddRange(new object[] { "Monthly", "Daily" });
                    cbRevenueType.SelectedIndex = 0;
                    cbRevenueType.SelectedIndexChanged += cbRevenueType_SelectedIndexChanged;

                    // Year ComboBox
                    var cbYear = new ComboBox
                    {
                        Location = new Point(170, 10),
                        Width = 100,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Name = "cbYear"
                    };
                    for (int year = DateTime.Now.Year - 5; year <= DateTime.Now.Year + 1; year++)
                    {
                        cbYear.Items.Add(year);
                    }
                    cbYear.SelectedItem = DateTime.Now.Year;

                    // DateTimePicker
                    var dtpDate = new DateTimePicker
                    {
                        Location = new Point(170, 10),
                        Width = 100,
                        Format = DateTimePickerFormat.Short,
                        Name = "dtpDate",
                        Visible = false
                    };
                    dtpDate.Value = DateTime.Now;

                    // Load Button
                    var btnLoadRevenue = new Button
                    {
                        Location = new Point(280, 10),
                        Width = 100,
                        Text = "Load Revenue"
                    };
                    btnLoadRevenue.Click += btnLoadRevenue_Click;

                    // DataGridView
                    var dgvRevenue = new DataGridView
                    {
                        Location = new Point(10, 40),
                        Size = new Size(tab.Width - 20, tab.Height - 50),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                        Name = "dgvRevenueGrid"
                    };

                    tab.Controls.AddRange(new Control[] { cbRevenueType, cbYear, dtpDate, btnLoadRevenue, dgvRevenue });
                    break;
                }
            }
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

                // Clear existing controls
                foreach (TabPage tab in tabControl1.TabPages)
                {
                    if (tab.Name == "tabInventory")
                        tab.Controls.Clear();
                    else if (tab.Name == "tabImports")
                        tab.Controls.Clear();
                    else if (tab.Name == "tabExports")
                        tab.Controls.Clear();
                }

                // Create and add new DataGridViews
                var dgvInventoryGrid = new DataGridView { Dock = DockStyle.Fill, Name = "dgvInventoryGrid" };
                var dgvImportReportGrid = new DataGridView { Dock = DockStyle.Fill, Name = "dgvImportReportGrid" };
                var dgvExportReportGrid = new DataGridView { Dock = DockStyle.Fill, Name = "dgvExportReportGrid" };

                foreach (TabPage tab in tabControl1.TabPages)
                {
                    if (tab.Name == "tabInventory")
                        tab.Controls.Add(dgvInventoryGrid);
                    else if (tab.Name == "tabImports")
                        tab.Controls.Add(dgvImportReportGrid);
                    else if (tab.Name == "tabExports")
                        tab.Controls.Add(dgvExportReportGrid);
                }

                dgvInventoryGrid.DataSource = inventory;
                dgvImportReportGrid.DataSource = imports;
                dgvExportReportGrid.DataSource = exports;

                ConfigureInventoryGrid(dgvInventoryGrid);
                ConfigureImportReportGrid(dgvImportReportGrid);
                ConfigureExportReportGrid(dgvExportReportGrid);

                // Load default revenue report (current year, monthly)
                if (_role.Equals("admin_role", StringComparison.OrdinalIgnoreCase))
                {
                    await LoadRevenueReportAsync(DateTime.Now.Year, true);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "loading reports");
            }
        }

        private async Task LoadRevenueReportAsync(int year, bool isMonthly)
        {
            try
            {
                List<RevenueReport> revenueData = null;
                DateTime? selectedDate = null;
                foreach (TabPage tab in tabControl1.TabPages)
                {
                    if (tab.Name == "tabRevenue")
                    {
                        var cbYear = tab.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cbYear");
                        var dtpDate = tab.Controls.OfType<DateTimePicker>().FirstOrDefault(c => c.Name == "dtpDate");
                        var dgvRevenue = tab.Controls.OfType<DataGridView>().FirstOrDefault(c => c.Name == "dgvRevenueGrid");

                        if (isMonthly)
                        {
                            if (cbYear != null && int.TryParse(cbYear.SelectedItem?.ToString(), out int selectedYear))
                            {
                                revenueData = await _reportRepository.GetRevenueByMonthAsync(selectedYear);
                            }
                        }
                        else
                        {
                            if (dtpDate != null)
                            {
                                selectedDate = dtpDate.Value;
                                revenueData = await _reportRepository.GetRevenueByDayAsync(selectedDate.Value);
                            }
                        }

                        if (dgvRevenue != null)
                        {
                            dgvRevenue.DataSource = revenueData;
                            ConfigureRevenueGrid(dgvRevenue, isMonthly);
                            if (revenueData == null || !revenueData.Any())
                            {
                                MessageBox.Show($"No revenue data found for {(isMonthly ? $"year {year}" : $"date {selectedDate?.ToString("yyyy-MM-dd")}")}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "loading revenue report");
            }
        }

        private void ConfigureInventoryGrid(DataGridView dgv)
        {
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            if (dgv.Columns.Contains("ProductId"))
                dgv.Columns["ProductId"].HeaderText = "ID";
            if (dgv.Columns.Contains("ProductName"))
                dgv.Columns["ProductName"].HeaderText = "Name";
            if (dgv.Columns.Contains("Price"))
            {
                dgv.Columns["Price"].HeaderText = "Price";
                dgv.Columns["Price"].DefaultCellStyle.Format = "N0";
            }
            if (dgv.Columns.Contains("StockQuantity"))
                dgv.Columns["StockQuantity"].HeaderText = "Stock Quantity";
            if (dgv.Columns.Contains("Brands"))
                dgv.Columns["Brands"].HeaderText = "Brands";

            if (dgv.Columns.Contains("Error")) dgv.Columns["Error"].Visible = false;
            if (dgv.Columns.Contains("HasError")) dgv.Columns["HasError"].Visible = false;
            if (dgv.Columns.Contains("ValidationErrors")) dgv.Columns["ValidationErrors"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureImportReportGrid(DataGridView dgv)
        {
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            if (dgv.Columns.Contains("ImportId"))
                dgv.Columns["ImportId"].HeaderText = "ID";
            if (dgv.Columns.Contains("SupplierId"))
                dgv.Columns["SupplierId"].Visible = false;
            if (dgv.Columns.Contains("SupplierName"))
                dgv.Columns["SupplierName"].HeaderText = "Supplier";
            if (dgv.Columns.Contains("ImportDate"))
            {
                dgv.Columns["ImportDate"].HeaderText = "Date";
                dgv.Columns["ImportDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
            }
            if (dgv.Columns.Contains("TotalAmount"))
            {
                dgv.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgv.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";
            }

            if (dgv.Columns.Contains("Error")) dgv.Columns["Error"].Visible = false;
            if (dgv.Columns.Contains("HasError")) dgv.Columns["HasError"].Visible = false;
            if (dgv.Columns.Contains("ValidationErrors")) dgv.Columns["ValidationErrors"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureExportReportGrid(DataGridView dgv)
        {
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            if (dgv.Columns.Contains("ExportId"))
                dgv.Columns["ExportId"].HeaderText = "ID";
            if (dgv.Columns.Contains("CustomerId"))
                dgv.Columns["CustomerId"].Visible = false;
            if (dgv.Columns.Contains("CustomerName"))
                dgv.Columns["CustomerName"].HeaderText = "Customer";
            if (dgv.Columns.Contains("ExportDate"))
            {
                dgv.Columns["ExportDate"].HeaderText = "Date";
                dgv.Columns["ExportDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
            }
            if (dgv.Columns.Contains("TotalAmount"))
            {
                dgv.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgv.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";
            }

            if (dgv.Columns.Contains("Error")) dgv.Columns["Error"].Visible = false;
            if (dgv.Columns.Contains("HasError")) dgv.Columns["HasError"].Visible = false;
            if (dgv.Columns.Contains("ValidationErrors")) dgv.Columns["ValidationErrors"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureRevenueGrid(DataGridView dgv, bool isMonthly)
        {
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            if (isMonthly)
            {
                if (dgv.Columns.Contains("Month"))
                    dgv.Columns["Month"].HeaderText = "Month";
                if (dgv.Columns.Contains("Date"))
                    dgv.Columns["Date"].Visible = false;
            }
            else
            {
                if (dgv.Columns.Contains("Date"))
                {
                    dgv.Columns["Date"].HeaderText = "Date";
                    dgv.Columns["Date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                }
                if (dgv.Columns.Contains("Month"))
                    dgv.Columns["Month"].Visible = false;
            }

            if (dgv.Columns.Contains("TotalRevenue"))
            {
                dgv.Columns["TotalRevenue"].HeaderText = "Total Revenue";
                dgv.Columns["TotalRevenue"].DefaultCellStyle.Format = "N0";
            }

            if (dgv.Columns.Contains("Error")) dgv.Columns["Error"].Visible = false;
            if (dgv.Columns.Contains("HasError")) dgv.Columns["HasError"].Visible = false;
            if (dgv.Columns.Contains("ValidationErrors")) dgv.Columns["ValidationErrors"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadReportsAsync();
        }

        private async void btnLoadRevenue_Click(object sender, EventArgs e)
        {
            foreach (TabPage tab in tabControl1.TabPages)
            {
                if (tab.Name == "tabRevenue")
                {
                    var cbRevenueType = tab.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name != "cbYear");
                    var cbYear = tab.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cbYear");
                    var dtpDate = tab.Controls.OfType<DateTimePicker>().FirstOrDefault(c => c.Name == "dtpDate");

                    bool isMonthly = cbRevenueType?.SelectedItem?.ToString() == "Monthly";
                    if (isMonthly)
                    {
                        if (cbYear != null && int.TryParse(cbYear.SelectedItem?.ToString(), out int year))
                        {
                            await LoadRevenueReportAsync(year, true);
                        }
                        else
                        {
                            MessageBox.Show("Please select a valid year.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        if (dtpDate != null)
                        {
                            await LoadRevenueReportAsync(0, false);
                        }
                        else
                        {
                            MessageBox.Show("Please select a valid date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    break;
                }
            }
        }

        private void cbRevenueType_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (TabPage tab in tabControl1.TabPages)
            {
                if (tab.Name == "tabRevenue")
                {
                    var cbRevenueType = (ComboBox)sender;
                    var cbYear = tab.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cbYear");
                    var dtpDate = tab.Controls.OfType<DateTimePicker>().FirstOrDefault(c => c.Name == "dtpDate");

                    if (cbRevenueType.SelectedItem?.ToString() == "Monthly")
                    {
                        if (cbYear != null) cbYear.Visible = true;
                        if (dtpDate != null) dtpDate.Visible = false;
                    }
                    else
                    {
                        if (cbYear != null) cbYear.Visible = false;
                        if (dtpDate != null) dtpDate.Visible = true;
                    }
                    break;
                }
            }
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            TabPage selectedTabPage = tabControl1.SelectedTab;
            if (selectedTabPage == null)
            {
                MessageBox.Show("No report tab selected.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                        using (ExcelPackage package = new ExcelPackage(excelFile))
                        {
                            string worksheetName = safeTabText.Length > 31 ? safeTabText.Substring(0, 31) : safeTabText;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

                            // Write Headers
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

                            // Write Data Rows
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
                                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
                                        }
                                        else if (actualValue is int || actualValue is long)
                                        {
                                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
                                        }
                                        else if (actualValue is DateTime dt)
                                        {
                                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yyyy-mm-dd";
                                            worksheet.Cells[rowIndex, colIndex].Value = dt;
                                        }

                                        colIndex++;
                                    }
                                }
                                rowIndex++;
                            }

                            // Auto-fit columns
                            if (worksheet.Dimension != null)
                            {
                                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            }

                            // Add formatting: borders and header background
                            if (worksheet.Dimension != null)
                            {
                                var tableRange = worksheet.Cells[1, 1, rowIndex - 1, colIndex - 1];
                                tableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                tableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                tableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                tableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells[1, 1, 1, colIndex - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, 1, 1, colIndex - 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            }

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
                        MessageBox.Show($"Error exporting data to Excel: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void HandleException(Exception ex, string action)
        {
            string message;
            string title;
            MessageBoxIcon icon;

            switch (ex)
            {
                case UnauthorizedAccessException:
                    message = ex.Message;
                    title = "Permission Denied";
                    icon = MessageBoxIcon.Error;
                    break;
                default:
                    message = $"Error {action}: {ex.Message}";
                    title = "Error";
                    icon = MessageBoxIcon.Error;
                    break;
            }

            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }
}