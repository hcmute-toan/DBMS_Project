using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class SupplierManagementForm : UserControl
    {
        private readonly SupplierRepository _supplierRepository;
        private readonly User _currentUser;

        public SupplierManagementForm(User currentUser)
        {
            InitializeComponent();
            _supplierRepository = new SupplierRepository();
            _currentUser = currentUser;
            LoadSuppliersAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvSuppliers.SelectionChanged += dgvSuppliers_SelectionChanged;
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var suppliers = await _supplierRepository.GetAllSuppliersAsync();
                dgvSuppliers.DataSource = suppliers;
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            if (dgvSuppliers.Columns.Contains("SupplierId"))
                dgvSuppliers.Columns["SupplierId"].HeaderText = "ID";
            if (dgvSuppliers.Columns.Contains("SupplierName"))
                dgvSuppliers.Columns["SupplierName"].HeaderText = "Name";
            if (dgvSuppliers.Columns.Contains("ContactInfo"))
                dgvSuppliers.Columns["ContactInfo"].HeaderText = "Contact Info";
            dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string supplierName = txtSupplierName.Text.Trim();
            string contactInfo = txtContactInfo.Text.Trim();

            if (string.IsNullOrEmpty(supplierName) || string.IsNullOrEmpty(contactInfo))
            {
                MessageBox.Show("Supplier name and contact info are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var supplier = new Supplier
                {
                    SupplierName = supplierName,
                    ContactInfo = contactInfo
                };
                int supplierId = await _supplierRepository.InsertSupplierAsync(_currentUser.UserId, supplier);
                MessageBox.Show($"Supplier added successfully with ID: {supplierId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadSuppliersAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string supplierName = txtSupplierName.Text.Trim();
            string contactInfo = txtContactInfo.Text.Trim();

            if (string.IsNullOrEmpty(supplierName) || string.IsNullOrEmpty(contactInfo))
            {
                MessageBox.Show("Supplier name and contact info are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedSupplier = (Supplier)dgvSuppliers.SelectedRows[0].DataBoundItem;
                var supplier = new Supplier
                {
                    SupplierId = selectedSupplier.SupplierId,
                    SupplierName = supplierName,
                    ContactInfo = contactInfo
                };
                await _supplierRepository.UpdateSupplierAsync(_currentUser.UserId, supplier);
                MessageBox.Show("Supplier updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadSuppliersAsync();
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
                MessageBox.Show($"Error updating supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this supplier?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedSupplier = (Supplier)dgvSuppliers.SelectedRows[0].DataBoundItem;
                    await _supplierRepository.DeleteSupplierAsync(_currentUser.UserId, selectedSupplier.SupplierId);
                    MessageBox.Show("Supplier deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadSuppliersAsync();
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
                    MessageBox.Show($"Error deleting supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadSuppliersAsync();
        }

        private void dgvSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count > 0)
            {
                var selectedSupplier = (Supplier)dgvSuppliers.SelectedRows[0].DataBoundItem;
                txtSupplierName.Text = selectedSupplier.SupplierName;
                txtContactInfo.Text = selectedSupplier.ContactInfo;
            }
        }

        private void ClearInputs()
        {
            txtSupplierName.Clear();
            txtContactInfo.Clear();
            dgvSuppliers.ClearSelection();
        }
    }
}