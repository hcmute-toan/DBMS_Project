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
        private readonly string _username;
        private readonly string _role;
        private readonly string _password;

        public SupplierManagementForm(string username, string role, string password)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            _password = password;
            _supplierRepository = new SupplierRepository(_username, _password);
            ConfigurePermissions();
            LoadSuppliersAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigurePermissions()
        {
            if (_role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
            {
                btnAdd.Enabled = false;
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
                HandleException(ex, "loading suppliers");
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
                int supplierId = await _supplierRepository.InsertSupplierAsync(supplier);
                MessageBox.Show($"Supplier added successfully with ID: {supplierId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadSuppliersAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex, "adding supplier");
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
                await _supplierRepository.UpdateSupplierAsync(supplier);
                MessageBox.Show("Supplier updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadSuppliersAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex, "updating supplier");
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
                    await _supplierRepository.DeleteSupplierAsync(selectedSupplier.SupplierId);
                    MessageBox.Show("Supplier deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadSuppliersAsync();
                }
                catch (Exception ex)
                {
                    HandleException(ex, "deleting supplier");
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
                case InvalidOperationException:
                    message = ex.Message;
                    title = "Operation Failed";
                    icon = MessageBoxIcon.Warning;
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