using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class ExportManagementForm : UserControl
    {
        private readonly ExportRepository _exportRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly ProductRepository _productRepository;
        private readonly User _currentUser;

        public ExportManagementForm(User currentUser)
        {
            InitializeComponent();
            _exportRepository = new ExportRepository();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _currentUser = currentUser;
            LoadCustomersAsync(); // Non-awaited in constructor
            LoadExportsAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvExports.SelectionChanged += dgvExports_SelectionChanged;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomersAsync(_currentUser.UserId);
                cboCustomer.DataSource = customers;
                cboCustomer.DisplayMember = "CustomerName";
                cboCustomer.ValueMember = "CustomerId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadExportsAsync()
        {
            try
            {
                var exports = await _exportRepository.GetAllExportsAsync();
                dgvExports.DataSource = exports;
                ConfigureExportDataGridView();
                if (exports.Any())
                {
                    await LoadExportDetailsAsync(exports.First().ExportId);
                }
                else
                {
                    dgvExportDetails.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading exports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadExportDetailsAsync(int exportId)
        {
            try
            {
                var details = await _exportRepository.GetExportDetailsAsync(exportId);
                dgvExportDetails.DataSource = details;
                ConfigureExportDetailsDataGridView();
                decimal totalAmount = await _exportRepository.GetExportTotalAsync(exportId);
                lblTotalAmount.Text = $"Total Amount: {totalAmount:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading export details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureExportDataGridView()
        {
            if (dgvExports.Columns.Contains("ExportId"))
                dgvExports.Columns["ExportId"].HeaderText = "ID";
            if (dgvExports.Columns.Contains("CustomerId"))
                dgvExports.Columns["CustomerId"].Visible = false;
            if (dgvExports.Columns.Contains("CustomerName"))
                dgvExports.Columns["CustomerName"].HeaderText = "Customer";
            if (dgvExports.Columns.Contains("ExportDate"))
                dgvExports.Columns["ExportDate"].HeaderText = "Date";
            if (dgvExports.Columns.Contains("TotalAmount"))
                dgvExports.Columns["TotalAmount"].HeaderText = "Total Amount";
            dgvExports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureExportDetailsDataGridView()
        {
            if (dgvExportDetails.Columns.Contains("ExportId"))
                dgvExportDetails.Columns["ExportId"].Visible = false;
            if (dgvExportDetails.Columns.Contains("ProductId"))
                dgvExportDetails.Columns["ProductId"].HeaderText = "Product ID";
            if (dgvExportDetails.Columns.Contains("ProductName"))
                dgvExportDetails.Columns["ProductName"].HeaderText = "Product";
            if (dgvExportDetails.Columns.Contains("Quantity"))
                dgvExportDetails.Columns["Quantity"].HeaderText = "Quantity";
            if (dgvExportDetails.Columns.Contains("UnitPrice"))
                dgvExportDetails.Columns["UnitPrice"].HeaderText = "Unit Price";
            dgvExportDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateExportInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var export = new Export
                {
                    CustomerId = (int)cboCustomer.SelectedValue,
                    ExportDate = dtpExportDate.Value,
                    TotalAmount = 0 // Will be calculated by details
                };
                int exportId = await _exportRepository.InsertExportAsync(_currentUser.UserId, export);

                var detail = new ExportDetail
                {
                    ExportId = exportId,
                    ProductId = 1, // Placeholder; ideally, select from a product list
                    Quantity = int.Parse(txtQuantity.Text.Trim()),
                    UnitPrice = decimal.Parse(txtUnitPrice.Text.Trim())
                };
                await _exportRepository.InsertExportDetailAsync(_currentUser.UserId, detail);

                MessageBox.Show($"Export added successfully with ID: {exportId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadExportsAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvExports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an export to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateExportInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedExport = (Export)dgvExports.SelectedRows[0].DataBoundItem;
                var export = new Export
                {
                    ExportId = selectedExport.ExportId,
                    CustomerId = (int)cboCustomer.SelectedValue,
                    ExportDate = dtpExportDate.Value,
                    TotalAmount = await _exportRepository.GetExportTotalAsync(selectedExport.ExportId)
                };
                await _exportRepository.UpdateExportAsync(_currentUser.UserId, export);
                MessageBox.Show("Export updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadExportsAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvExports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an export to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this export?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedExport = (Export)dgvExports.SelectedRows[0].DataBoundItem;
                    await _exportRepository.DeleteExportAsync(_currentUser.UserId, selectedExport.ExportId);
                    MessageBox.Show("Export deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadExportsAsync();
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (KeyNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadCustomersAsync();
            await LoadExportsAsync();
        }

        private async void dgvExports_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvExports.SelectedRows.Count > 0)
            {
                var selectedExport = (Export)dgvExports.SelectedRows[0].DataBoundItem;
                cboCustomer.SelectedValue = selectedExport.CustomerId;
                dtpExportDate.Value = selectedExport.ExportDate;
                await LoadExportDetailsAsync(selectedExport.ExportId);
            }
        }

        private bool ValidateExportInputs(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (cboCustomer.SelectedValue == null)
                errorMessage = "Please select a customer.";
            else if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity) || quantity <= 0)
                errorMessage = "Quantity must be a positive integer.";
            else if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out decimal unitPrice) || unitPrice <= 0)
                errorMessage = "Unit price must be a positive number.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void ClearInputs()
        {
            cboCustomer.SelectedIndex = -1;
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            dtpExportDate.Value = DateTime.Now;
            lblTotalAmount.Text = "Total Amount: ";
            dgvExports.ClearSelection();
            dgvExportDetails.DataSource = null;
        }

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle customer selection if needed
        }
    }
}