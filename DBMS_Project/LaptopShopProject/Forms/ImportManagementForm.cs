using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class ImportManagementForm : UserControl
    {
        private readonly ImportRepository _importRepository;
        private readonly SupplierRepository _supplierRepository;
        private readonly User _currentUser;

        public ImportManagementForm(User currentUser)
        {
            InitializeComponent();
            _importRepository = new ImportRepository();
            _supplierRepository = new SupplierRepository();
            _currentUser = currentUser;
            LoadSuppliersAsync(); // Non-awaited in constructor
            LoadImportsAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvImports.SelectionChanged += dgvImports_SelectionChanged;
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var suppliers = await _supplierRepository.GetAllSuppliersAsync(_currentUser.UserId);
                cboSupplier.DataSource = suppliers;
                cboSupplier.DisplayMember = "SupplierName";
                cboSupplier.ValueMember = "SupplierId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadImportsAsync()
        {
            try
            {
                var imports = await _importRepository.GetAllImportsAsync();
                dgvImports.DataSource = imports;
                ConfigureImportDataGridView();
                if (imports.Any())
                {
                    await LoadImportDetailsAsync(imports.First().ImportId);
                }
                else
                {
                    dgvImportDetails.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading imports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadImportDetailsAsync(int importId)
        {
            try
            {
                var details = await _importRepository.GetImportDetailsAsync(importId);
                dgvImportDetails.DataSource = details;
                ConfigureImportDetailsDataGridView();
                decimal totalAmount = await _importRepository.GetImportTotalAsync(importId);
                lblTotalAmount.Text = $"Total Amount: {totalAmount:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading import details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureImportDataGridView()
        {
            if (dgvImports.Columns.Contains("ImportId"))
                dgvImports.Columns["ImportId"].HeaderText = "ID";
            if (dgvImports.Columns.Contains("SupplierId"))
                dgvImports.Columns["SupplierId"].Visible = false;
            if (dgvImports.Columns.Contains("SupplierName"))
                dgvImports.Columns["SupplierName"].HeaderText = "Supplier";
            if (dgvImports.Columns.Contains("ImportDate"))
                dgvImports.Columns["ImportDate"].HeaderText = "Date";
            if (dgvImports.Columns.Contains("TotalAmount"))
                dgvImports.Columns["TotalAmount"].HeaderText = "Total Amount";
            dgvImports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureImportDetailsDataGridView()
        {
            if (dgvImportDetails.Columns.Contains("ImportId"))
                dgvImportDetails.Columns["ImportId"].Visible = false;
            if (dgvImportDetails.Columns.Contains("ProductId"))
                dgvImportDetails.Columns["ProductId"].HeaderText = "Product ID";
            if (dgvImportDetails.Columns.Contains("ProductName"))
                dgvImportDetails.Columns["ProductName"].HeaderText = "Product";
            if (dgvImportDetails.Columns.Contains("Quantity"))
                dgvImportDetails.Columns["Quantity"].HeaderText = "Quantity";
            if (dgvImportDetails.Columns.Contains("UnitPrice"))
                dgvImportDetails.Columns["UnitPrice"].HeaderText = "Unit Price";
            dgvImportDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateImportInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var import = new Import
                {
                    SupplierId = (int)cboSupplier.SelectedValue,
                    ImportDate = dtpImportDate.Value,
                    TotalAmount = 0 // Will be calculated by details
                };
                int importId = await _importRepository.InsertImportAsync(_currentUser.UserId, import);

                var detail = new ImportDetail
                {
                    ImportId = importId,
                    ProductName = txtProductName.Text.Trim(),
                    Quantity = int.Parse(txtQuantity.Text.Trim()),
                    UnitPrice = decimal.Parse(txtUnitPrice.Text.Trim())
                };
                decimal? price = string.IsNullOrEmpty(txtPrice.Text.Trim()) ? (decimal?)null : decimal.Parse(txtPrice.Text.Trim());
                int productId = await _importRepository.InsertImportDetailAsync(_currentUser.UserId, detail, price);

                MessageBox.Show($"Import added successfully with ID: {importId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadImportsAsync();
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
                MessageBox.Show($"Error adding import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an import to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateImportInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                var import = new Import
                {
                    ImportId = selectedImport.ImportId,
                    SupplierId = (int)cboSupplier.SelectedValue,
                    ImportDate = dtpImportDate.Value,
                    TotalAmount = await _importRepository.GetImportTotalAsync(selectedImport.ImportId)
                };
                await _importRepository.UpdateImportAsync(_currentUser.UserId, import);
                MessageBox.Show("Import updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadImportsAsync();
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
                MessageBox.Show($"Error updating import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an import to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this import?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                    await _importRepository.DeleteImportAsync(_currentUser.UserId, selectedImport.ImportId);
                    MessageBox.Show("Import deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadImportsAsync();
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
                    MessageBox.Show($"Error deleting import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadSuppliersAsync();
            await LoadImportsAsync();
        }

        private async void dgvImports_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count > 0)
            {
                var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                cboSupplier.SelectedValue = selectedImport.SupplierId;
                dtpImportDate.Value = selectedImport.ImportDate;
                await LoadImportDetailsAsync(selectedImport.ImportId);
            }
        }

        private bool ValidateImportInputs(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (cboSupplier.SelectedValue == null)
                errorMessage = "Please select a supplier.";
            else if (string.IsNullOrEmpty(txtProductName.Text.Trim()))
                errorMessage = "Product name is required.";
            else if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity) || quantity <= 0)
                errorMessage = "Quantity must be a positive integer.";
            else if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out decimal unitPrice) || unitPrice <= 0)
                errorMessage = "Unit price must be a positive number.";
            else if (!string.IsNullOrEmpty(txtPrice.Text.Trim()) && (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price <= 0))
                errorMessage = "Price must be a positive number or empty.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void ClearInputs()
        {
            cboSupplier.SelectedIndex = -1;
            txtProductName.Clear();
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            txtPrice.Clear();
            dtpImportDate.Value = DateTime.Now;
            lblTotalAmount.Text = "Total Amount: ";
            dgvImports.ClearSelection();
            dgvImportDetails.DataSource = null;
        }
    }
}