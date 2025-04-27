using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class CustomerManagementForm : UserControl
    {
        private readonly CustomerRepository _customerRepository;
        private readonly User _currentUser;

        public CustomerManagementForm(User currentUser)
        {
            InitializeComponent();
            _customerRepository = new CustomerRepository();
            _currentUser = currentUser;
            LoadCustomersAsync(); // Non-awaited call in constructor
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomersAsync();
                dgvCustomers.DataSource = customers;
                ConfigureDataGridView();
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
                int customerId = await _customerRepository.InsertCustomerAsync(_currentUser.UserId, customer);
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
                await _customerRepository.UpdateCustomerAsync(_currentUser.UserId, customer);
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
                    await _customerRepository.DeleteCustomerAsync(_currentUser.UserId, selectedCustomer.CustomerId);
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
                txtContactInfo.Text = selectedCustomer.ContactInfo;
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