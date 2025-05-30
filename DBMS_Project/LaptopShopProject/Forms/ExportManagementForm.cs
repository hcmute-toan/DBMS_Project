﻿using LaptopShopProject.DataAccess;
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
        private readonly string _username;
        private readonly string _role;
        private readonly string _password; // Temporary; replace with secure password handling

        public ExportManagementForm(string username, string role)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            // Temporary password; in production, use secure storage or prompt
            _password = username == "admin_user" ? "Admin123!" : "Employee123!";
            _exportRepository = new ExportRepository(_username, _password);
            _customerRepository = new CustomerRepository(_username, _password);
            _productRepository = new ProductRepository(_username, _password);
            ConfigurePermissions();
            LoadCustomersAsync(); // Non-awaited in constructor
            LoadExportsAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigurePermissions()
        {
            if (_role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
            {
                // Disable buttons for employee_role since they only have INSERT permissions
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
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
                var customers = await _customerRepository.GetAllCustomersAsync();
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
                // Ensure TotalAmount is correct for each Export
                foreach (var export in exports)
                {
                    export.TotalAmount = await _exportRepository.GetExportTotalAsync(export.ExportId);
                }
                dgvExports.DataSource = null; // Clear existing data
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
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false; // Disable the entire form if no permission
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
                lblTotalAmount.Text = $"Total Amount: {totalAmount:N0} đ"; // Format for Vietnamese Dong
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
            {
                dgvExports.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgvExports.Columns["TotalAmount"].DefaultCellStyle.Format = "N0"; // Format for Vietnamese Dong
            }
            dgvExports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureExportDetailsDataGridView()
        {
            if (dgvExportDetails.Columns.Contains("ExportId"))
                dgvExportDetails.Columns["ExportId"].Visible = false;
            if (dgvExportDetails.Columns.Contains("ProductId"))
                dgvExportDetails.Columns["ProductId"].Visible = false; // Hide ProductId since we're using ProductName
            if (dgvExportDetails.Columns.Contains("ProductName"))
                dgvExportDetails.Columns["ProductName"].HeaderText = "Product";
            if (dgvExportDetails.Columns.Contains("Quantity"))
                dgvExportDetails.Columns["Quantity"].HeaderText = "Quantity";
            if (dgvExportDetails.Columns.Contains("UnitPrice"))
            {
                dgvExportDetails.Columns["UnitPrice"].HeaderText = "Unit Price";
                dgvExportDetails.Columns["UnitPrice"].DefaultCellStyle.Format = "N0"; // Format for Vietnamese Dong
            }
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
                string productName = txtProductName.Text.Trim();
                bool productExists = await _productRepository.ProductExistsAsync(productName);

                if (!productExists)
                {
                    MessageBox.Show($"Product '{productName}' does not exist. Please add the product first.", "Product Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert the Export
                var export = new Export
                {
                    CustomerId = (int)cboCustomer.SelectedValue,
                    ExportDate = dtpExportDate.Value,
                    TotalAmount = 0
                };
                int exportId = await _exportRepository.InsertExportAsync(export);

                // Insert the ExportDetail using ProductName
                var detail = new ExportDetail
                {
                    ExportId = exportId,
                    ProductName = productName,
                    Quantity = int.Parse(txtQuantity.Text.Trim()),
                    UnitPrice = decimal.Parse(txtUnitPrice.Text.Trim())
                };
                await _exportRepository.InsertExportDetailAsync(detail);

                MessageBox.Show($"Export added successfully with ID: {exportId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadExportsAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex, "adding export");
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvExports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an export to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvExportDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an export detail to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                var selectedDetail = (ExportDetail)dgvExportDetails.SelectedRows[0].DataBoundItem;

                string productName = txtProductName.Text.Trim();
                bool productExists = await _productRepository.ProductExistsAsync(productName);

                if (!productExists)
                {
                    MessageBox.Show($"Product '{productName}' does not exist. Please add the product first.", "Product Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int newQuantity = int.Parse(txtQuantity.Text.Trim());
                decimal newUnitPrice = decimal.Parse(txtUnitPrice.Text.Trim());

                // Update ExportDetail
                var detail = new ExportDetail
                {
                    ExportId = selectedExport.ExportId,
                    ProductName = productName,
                    Quantity = newQuantity,
                    UnitPrice = newUnitPrice
                };
                bool isUpdated = await _exportRepository.UpdateExportDetailAsync(detail);

                if (isUpdated)
                {
                    MessageBox.Show("Export updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No changes detected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Refresh UI
                await LoadExportsAsync();

                // Reselect the updated row
                foreach (DataGridViewRow row in dgvExports.Rows)
                {
                    var currentExport = (Export)row.DataBoundItem;
                    if (currentExport.ExportId == selectedExport.ExportId)
                    {
                        row.Selected = true;
                        await LoadExportDetailsAsync(selectedExport.ExportId);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "updating export");
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
                    await _exportRepository.DeleteExportAsync(selectedExport.ExportId);
                    MessageBox.Show("Export deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadExportsAsync();
                }
                catch (Exception ex)
                {
                    HandleException(ex, "deleting export");
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

                // Populate text fields with the first ExportDetail (if any)
                var details = await _exportRepository.GetExportDetailsAsync(selectedExport.ExportId);
                if (details.Any())
                {
                    var firstDetail = details.First();
                    txtProductName.Text = firstDetail.ProductName;
                    txtQuantity.Text = firstDetail.Quantity.ToString();
                    txtUnitPrice.Text = firstDetail.UnitPrice.ToString("N0"); // Format without currency symbol
                }
                else
                {
                    txtProductName.Clear();
                    txtQuantity.Clear();
                    txtUnitPrice.Clear();
                }
            }
        }

        private bool ValidateExportInputs(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (cboCustomer.SelectedValue == null)
                errorMessage = "Please select a customer.";
            else if (string.IsNullOrEmpty(txtProductName.Text.Trim()))
                errorMessage = "Product name is required.";
            else if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity) || quantity <= 0)
                errorMessage = "Quantity must be a positive integer.";
            else if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out decimal unitPrice) || unitPrice <= 0)
                errorMessage = "Unit price must be a positive number.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void ClearInputs()
        {
            cboCustomer.SelectedIndex = -1;
            txtProductName.Clear();
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            dtpExportDate.Value = DateTime.Now;
            lblTotalAmount.Text = "Total Amount: ";
            dgvExports.ClearSelection();
            dgvExportDetails.DataSource = null;
        }

        private void HandleException(Exception ex, string action)
        {
            string message;
            string title;
            MessageBoxIcon icon;

            switch (ex)
            {
                case InvalidOperationException:
                    message = ex.Message;
                    title = "Operation Failed";
                    icon = MessageBoxIcon.Warning;
                    break;
                case UnauthorizedAccessException:
                    message = ex.Message;
                    title = "Permission Denied";
                    icon = MessageBoxIcon.Error;
                    break;
                case KeyNotFoundException:
                    message = ex.Message;
                    title = "Not Found";
                    icon = MessageBoxIcon.Warning;
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