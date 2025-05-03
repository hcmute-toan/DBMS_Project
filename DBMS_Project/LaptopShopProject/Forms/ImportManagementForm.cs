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
        private readonly ProductRepository _productRepository;
        private readonly string _username;
        private readonly string _role;
        private readonly string _password;

        public ImportManagementForm(string username, string role, string password)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            _password = password;
            _importRepository = new ImportRepository(_username, _password);
            _supplierRepository = new SupplierRepository(_username, _password);
            _productRepository = new ProductRepository(_username, _password);
            ConfigurePermissions();
            LoadSuppliersAsync();
            LoadImportsAsync();
            ConfigureEventHandlers();
        }

        private void ConfigurePermissions()
        {
            if (_role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
            {
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
            dgvImports.SelectionChanged += dgvImports_SelectionChanged;
            dgvImportDetails.SelectionChanged += dgvImportDetails_SelectionChanged;
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var suppliers = await _supplierRepository.GetAllSuppliersAsync();
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
                foreach (var import in imports)
                {
                    import.TotalAmount = await _importRepository.GetImportTotalAsync(import.ImportId);
                }
                dgvImports.DataSource = null;
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
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false;
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
                lblTotalAmount.Text = $"Total Amount: {totalAmount:N0} đ";
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
            {
                dgvImports.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgvImports.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";
            }
            dgvImports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureImportDetailsDataGridView()
        {
            if (dgvImportDetails.Columns.Contains("ImportId"))
                dgvImportDetails.Columns["ImportId"].Visible = false;
            if (dgvImportDetails.Columns.Contains("ProductId"))
                dgvImportDetails.Columns["ProductId"].Visible = false;
            if (dgvImportDetails.Columns.Contains("ProductName"))
                dgvImportDetails.Columns["ProductName"].HeaderText = "Product";
            if (dgvImportDetails.Columns.Contains("Quantity"))
                dgvImportDetails.Columns["Quantity"].HeaderText = "Quantity";
            if (dgvImportDetails.Columns.Contains("UnitPrice"))
            {
                dgvImportDetails.Columns["UnitPrice"].HeaderText = "Unit Price";
                dgvImportDetails.Columns["UnitPrice"].DefaultCellStyle.Format = "N0";
            }
            dgvImportDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateImportInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Please enter a product name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Please enter a valid unit price greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal? price = null;
            if (!string.IsNullOrWhiteSpace(txtPrice.Text) && decimal.TryParse(txtPrice.Text, out decimal parsedPrice) && parsedPrice > 0)
            {
                price = parsedPrice;
            }

            string categoryName = string.IsNullOrWhiteSpace(txtCategory.Text) ? null : txtCategory.Text.Trim();
            string categoryDescription = string.IsNullOrWhiteSpace(txtCategoryDescription.Text) ? null : txtCategoryDescription.Text.Trim();

            try
            {
                int supplierId = (int)cboSupplier.SelectedValue;
                DateTime importDate = dtpImportDate.Value.Date;

                // Check if product exists
                bool productExists = await _productRepository.ProductExistsAsync(txtProductName.Text.Trim());
                if (!productExists && _role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Product '{txtProductName.Text}' does not exist. Please add the product first.", "Product Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create new import
                var import = new Import
                {
                    SupplierId = supplierId,
                    ImportDate = importDate,
                    TotalAmount = 0
                };
                int importId = await _importRepository.InsertImportAsync(import);

                // Insert import detail
                var detail = new ImportDetail
                {
                    ImportId = importId,
                    ProductName = txtProductName.Text.Trim(),
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };
                await _importRepository.InsertImportDetailAsync(detail, price, categoryName, categoryDescription);

                MessageBox.Show($"Import added successfully with ID: {importId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadImportsAsync();
                foreach (DataGridViewRow row in dgvImports.Rows)
                {
                    var importRow = (Import)row.DataBoundItem;
                    if (importRow.ImportId == importId)
                    {
                        row.Selected = true;
                        await LoadImportDetailsAsync(importId);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "adding import");
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an import to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvImportDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an import detail to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                var selectedDetail = (ImportDetail)dgvImportDetails.SelectedRows[0].DataBoundItem;

                if (!int.TryParse(txtQuantity.Text, out int newQuantity) || newQuantity <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtUnitPrice.Text, out decimal newUnitPrice) || newUnitPrice <= 0)
                {
                    MessageBox.Show("Please enter a valid unit price greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal newPrice;
                if (!string.IsNullOrWhiteSpace(txtPrice.Text) && decimal.TryParse(txtPrice.Text, out decimal parsedPrice) && parsedPrice > 0)
                {
                    newPrice = parsedPrice;
                }
                else
                {
                    var product = await _productRepository.GetProductByNameAsync(selectedDetail.ProductName);
                    newPrice = product?.Price ?? selectedDetail.UnitPrice;
                }

                var updatedDetail = new ImportDetail
                {
                    ImportId = selectedImport.ImportId,
                    ProductName = selectedDetail.ProductName,
                    Quantity = newQuantity,
                    UnitPrice = newUnitPrice
                };
                bool isUpdated = await _importRepository.UpdateImportAsync(updatedDetail, newPrice);

                if (isUpdated)
                {
                    MessageBox.Show("Import updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No changes detected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                await LoadImportsAsync();
                foreach (DataGridViewRow row in dgvImports.Rows)
                {
                    var currentImport = (Import)row.DataBoundItem;
                    if (currentImport.ImportId == selectedImport.ImportId)
                    {
                        row.Selected = true;
                        await LoadImportDetailsAsync(selectedImport.ImportId);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "updating import");
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
                    await _importRepository.DeleteImportAsync(selectedImport.ImportId);
                    MessageBox.Show("Import deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadImportsAsync();
                }
                catch (Exception ex)
                {
                    HandleException(ex, "deleting import");
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadSuppliersAsync();
            await LoadImportsAsync();
        }

        private void dgvImports_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count > 0)
            {
                var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                cboSupplier.SelectedValue = selectedImport.SupplierId;
                dtpImportDate.Value = selectedImport.ImportDate;
                LoadImportDetailsAsync(selectedImport.ImportId);
                ClearDetailInputs();
            }
        }

        private async void dgvImportDetails_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private bool ValidateImportInputs(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (cboSupplier.SelectedValue == null)
                errorMessage = "Please select a supplier.";
            else if (dtpImportDate.Value > DateTime.Now)
                errorMessage = "Import date cannot be in the future.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void ClearInputs()
        {
            cboSupplier.SelectedIndex = -1;
            dtpImportDate.Value = DateTime.Now;
            lblTotalAmount.Text = "Total Amount: ";
            dgvImports.ClearSelection();
            dgvImportDetails.DataSource = null;
            ClearDetailInputs();
        }

        private void ClearDetailInputs()
        {
            txtProductName.Clear();
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            txtPrice.Clear();
            txtCategory.Clear();
            txtCategoryDescription.Clear();
            dgvImportDetails.ClearSelection();
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