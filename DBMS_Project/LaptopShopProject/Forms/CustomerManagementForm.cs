using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class CustomerManagementForm : UserControl
    {
        private readonly CustomerRepository _customerRepository;
        private readonly string _username;
        private readonly string _role;
        private readonly string _password; // Temporary; replace with secure password handling

        public CustomerManagementForm(string username, string role)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            // Temporary password; in production, use secure storage or prompt
            _password = username == "admin_user" ? "Admin123!" : "Employee123!";
            _customerRepository = new CustomerRepository(_username, _password);
            ConfigurePermissions();
            LoadCustomersAsync(); // Non-awaited call in constructor
            ConfigureEventHandlers();
        }

        private void ConfigurePermissions()
        {
            if (_role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
            {
                // Disable buttons for employee_role since they only have SELECT permission
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                txtCustomerName.ReadOnly = true;
                txtContactInfo.ReadOnly = true;
            }
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvCustomers.SelectionChanged += dgvCustomers_SelectionChanged;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomersAsync();
                dgvCustomers.DataSource = customers;
                ConfigureDataGridView();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false; // Disable the entire form if no permission
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            if (dgvCustomers.Columns.Contains("CustomerId"))
                dgvCustomers.Columns["CustomerId"].HeaderText = "ID";
            if (dgvCustomers.Columns.Contains("CustomerName"))
                dgvCustomers.Columns["CustomerName"].HeaderText = "Name";
            if (dgvCustomers.Columns.Contains("ContactInfo"))
                dgvCustomers.Columns["ContactInfo"].HeaderText = "Contact Info";
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string customerName = txtCustomerName.Text.Trim();
            string contactInfo = txtContactInfo.Text.Trim();

            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(contactInfo))
            {
                MessageBox.Show("Customer name and contact info are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var customer = new Customer
                {
                    CustomerName = customerName,
                    ContactInfo = contactInfo
                };
                int customerId = await _customerRepository.InsertCustomerAsync(customer);
                MessageBox.Show($"Customer added successfully with ID: {customerId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadCustomersAsync();
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
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerName = txtCustomerName.Text.Trim();
            string contactInfo = txtContactInfo.Text.Trim();

            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(contactInfo))
            {
                MessageBox.Show("Customer name and contact info are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedCustomer = (Customer)dgvCustomers.SelectedRows[0].DataBoundItem;
                var customer = new Customer
                {
                    CustomerId = selectedCustomer.CustomerId,
                    CustomerName = customerName,
                    ContactInfo = contactInfo
                };
                await _customerRepository.UpdateCustomerAsync(customer);
                MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadCustomersAsync();
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
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedCustomer = (Customer)dgvCustomers.SelectedRows[0].DataBoundItem;
                    await _customerRepository.DeleteCustomerAsync(selectedCustomer.CustomerId);
                    MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadCustomersAsync();
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
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadCustomersAsync();
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                var selectedCustomer = (Customer)dgvCustomers.SelectedRows[0].DataBoundItem;
                txtCustomerName.Text = selectedCustomer.CustomerName;
                txtContactInfo.Text = selectedCustomer.ContactInfo ?? string.Empty;
            }
        }

        private void ClearInputs()
        {
            txtCustomerName.Clear();
            txtContactInfo.Clear();
            dgvCustomers.ClearSelection();
        }
    }
}