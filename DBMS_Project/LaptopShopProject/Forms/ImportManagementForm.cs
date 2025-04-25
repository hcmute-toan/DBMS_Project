using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class ImportManagementForm : UserControl
    {
        private readonly ImportRepository _importRepository;
        private readonly SupplierRepository _supplierRepository;
        private readonly ProductRepository _productRepository;
        private int _currentUserId; // Set this based on logged-in user
        private int _selectedImportId = -1;

        public ImportManagementForm(int currentUserId)
        {
            InitializeComponent();
            _importRepository = new ImportRepository();
            _supplierRepository = new SupplierRepository();
            _productRepository = new ProductRepository();
            _currentUserId = currentUserId;
            InitializeForm();
        }

        private void InitializeForm()
        {
            LoadSuppliers();
            LoadImports();
            ConfigureDataGridViews();
            ClearInputFields();
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void ConfigureDataGridViews()
        {
            // Configure dgvImports
            dgvImports.AutoGenerateColumns = false;
            dgvImports.Columns.Clear();
            dgvImports.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ImportId", HeaderText = "Import ID" });
            dgvImports.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Supplier" });
            dgvImports.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ImportDate", HeaderText = "Import Date" });
            dgvImports.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalAmount", HeaderText = "Total Amount" });

            // Configure dgvImportDetails
            dgvImportDetails.AutoGenerateColumns = false;
            dgvImportDetails.Columns.Clear();
            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "Product ID" });
            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Product Name" });
            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Quantity" });
            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Unit Price" });
        }

        private void LoadSuppliers()
        {
            try
            {
                var suppliers = _supplierRepository.GetAllSuppliers(_currentUserId);
                cboSupplier.DataSource = suppliers;
                cboSupplier.DisplayMember = "SupplierName";
                cboSupplier.ValueMember = "SupplierId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadImports()
        {
            try
            {
                var imports = _importRepository.GetAllImports();
                dgvImports.DataSource = imports;
                if (imports.Count > 0)
                {
                    _selectedImportId = imports[0].ImportId;
                    LoadImportDetails(_selectedImportId);
                }
                else
                {
                    dgvImportDetails.DataSource = null;
                    lblTotalAmount.Text = "Total Amount: 0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading imports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadImportDetails(int importId)
        {
            try
            {
                var details = _importRepository.GetImportDetails(importId);
                dgvImportDetails.DataSource = details;
                decimal totalAmount = _importRepository.GetImportTotal(importId);
                lblTotalAmount.Text = $"Total Amount: {totalAmount:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading import details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputFields()
        {
            txtProductName.Text = "";
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
            txtPrice.Text = "";
            dtpImportDate.Value = DateTime.Now;
            cboSupplier.SelectedIndex = -1;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("Product name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Quantity must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
                {
                    MessageBox.Show("Unit price must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cboSupplier.SelectedValue == null)
                {
                    MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal? price = null;
                if (!string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    if (!decimal.TryParse(txtPrice.Text, out decimal parsedPrice) || parsedPrice <= 0)
                    {
                        MessageBox.Show("Price must be a positive number if provided.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    price = parsedPrice;
                }

                // Create new import if no import is selected or create a new one
                if (_selectedImportId == -1)
                {
                    var import = new Import
                    {
                        SupplierId = (int)cboSupplier.SelectedValue,
                        ImportDate = dtpImportDate.Value,
                        TotalAmount = 0 // Will be updated after adding details
                    };
                    _selectedImportId = _importRepository.InsertImport(_currentUserId, import);
                }

                // Add import detail
                var detail = new ImportDetail
                {
                    ImportId = _selectedImportId,
                    ProductName = txtProductName.Text.Trim(),
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };
                _importRepository.InsertImportDetail(_currentUserId, detail, price);

                // Update total amount
                decimal totalAmount = _importRepository.GetImportTotal(_selectedImportId);
                var updatedImport = new Import
                {
                    ImportId = _selectedImportId,
                    SupplierId = (int)cboSupplier.SelectedValue,
                    ImportDate = dtpImportDate.Value,
                    TotalAmount = totalAmount
                };
                _importRepository.UpdateImport(_currentUserId, updatedImport);

                MessageBox.Show("Import detail added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadImports();
                LoadImportDetails(_selectedImportId);
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedImportId == -1)
                {
                    MessageBox.Show("Please select an import to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cboSupplier.SelectedValue == null)
                {
                    MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var import = new Import
                {
                    ImportId = _selectedImportId,
                    SupplierId = (int)cboSupplier.SelectedValue,
                    ImportDate = dtpImportDate.Value,
                    TotalAmount = _importRepository.GetImportTotal(_selectedImportId)
                };
                _importRepository.UpdateImport(_currentUserId, import);

                MessageBox.Show("Import updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadImports();
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedImportId == -1)
                {
                    MessageBox.Show("Please select an import to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to delete this import?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _importRepository.DeleteImport(_currentUserId, _selectedImportId);
                    MessageBox.Show("Import deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _selectedImportId = -1;
                    LoadImports();
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
            LoadImports();
            ClearInputFields();
            _selectedImportId = -1;
        }

        private void dgvImports_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count > 0)
            {
                var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                _selectedImportId = selectedImport.ImportId;
                cboSupplier.SelectedValue = selectedImport.SupplierId;
                dtpImportDate.Value = selectedImport.ImportDate;
                LoadImportDetails(_selectedImportId);
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                _selectedImportId = -1;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }
    }
}