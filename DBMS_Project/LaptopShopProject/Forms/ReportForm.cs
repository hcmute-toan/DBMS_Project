using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class ReportForm : UserControl
    {
        private readonly ReportRepository _reportRepository;
        private readonly User _currentUser;

        public ReportForm(User currentUser)
        {
            InitializeComponent();
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

                var dgvInventoryGrid = new DataGridView { Dock = DockStyle.Fill };
                var dgvImportReportGrid = new DataGridView { Dock = DockStyle.Fill };
                var dgvExportReportGrid = new DataGridView { Dock = DockStyle.Fill };

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
            if (dgv.Columns.Contains("ProductId"))
                dgv.Columns["ProductId"].HeaderText = "ID";
            if (dgv.Columns.Contains("ProductName"))
                dgv.Columns["ProductName"].HeaderText = "Name";
            if (dgv.Columns.Contains("Price"))
                dgv.Columns["Price"].HeaderText = "Price";
            if (dgv.Columns.Contains("StockQuantity"))
                dgv.Columns["StockQuantity"].HeaderText = "Stock Quantity";
            if (dgv.Columns.Contains("Brands"))
                dgv.Columns["Brands"].HeaderText = "Brands";
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureImportReportGrid(DataGridView dgv)
        {
            if (dgv.Columns.Contains("ImportId"))
                dgv.Columns["ImportId"].HeaderText = "ID";
            if (dgv.Columns.Contains("SupplierId"))
                dgv.Columns["SupplierId"].Visible = false;
            if (dgv.Columns.Contains("SupplierName"))
                dgv.Columns["SupplierName"].HeaderText = "Supplier";
            if (dgv.Columns.Contains("ImportDate"))
                dgv.Columns["ImportDate"].HeaderText = "Date";
            if (dgv.Columns.Contains("TotalAmount"))
                dgv.Columns["TotalAmount"].HeaderText = "Total Amount";
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureExportReportGrid(DataGridView dgv)
        {
            if (dgv.Columns.Contains("ExportId"))
                dgv.Columns["ExportId"].HeaderText = "ID";
            if (dgv.Columns.Contains("CustomerId"))
                dgv.Columns["CustomerId"].Visible = false;
            if (dgv.Columns.Contains("CustomerName"))
                dgv.Columns["CustomerName"].HeaderText = "Customer";
            if (dgv.Columns.Contains("ExportDate"))
                dgv.Columns["ExportDate"].HeaderText = "Date";
            if (dgv.Columns.Contains("TotalAmount"))
                dgv.Columns["TotalAmount"].HeaderText = "Total Amount";
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadReportsAsync();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Export to Excel functionality not implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // TODO: Implement Excel export using a library like EPPlus
        }
    }
}