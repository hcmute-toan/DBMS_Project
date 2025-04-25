using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class CustomerManagementForm : UserControl
    {
        private readonly User _currentUser;
        private readonly CustomerRepository _customerRepository;
        private int _selectedCustomerId = 0;

        public CustomerManagementForm(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _customerRepository = new CustomerRepository();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerRepository.GetAllCustomers(_currentUser.UserId);
                dgvCustomers.DataSource = customers;

                // Configure DataGridView columns
                dgvCustomers.Columns["CustomerId"].HeaderText = "ID";
                dgvCustomers.Columns["CustomerName"].HeaderText = "Customer Name";
                dgvCustomers.Columns["ContactInfo"].HeaderText = "Contact Info";

                // Clear selection
                dgvCustomers.ClearSelection();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtCustomerName.Text = string.Empty;
            txtContactInfo.Text = string.Empty;
            _selectedCustomerId = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("Customer name is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtContactInfo.Text))
                {
                    MessageBox.Show("Contact information is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var customer = new Customer
                {
                    CustomerName = txtCustomerName.Text.Trim(),
                    ContactInfo = txtContactInfo.Text.Trim()
                };

                _customerRepository.InsertCustomer(_currentUser.UserId, customer);
                MessageBox.Show("Customer added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId == 0)
                {
                    MessageBox.Show("Please select a customer to update.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("Customer name is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtContactInfo.Text))
                {
                    MessageBox.Show("Contact information is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var customer = new Customer
                {
                    CustomerId = _selectedCustomerId,
                    CustomerName = txtCustomerName.Text.Trim(),
                    ContactInfo = txtContactInfo.Text.Trim()
                };

                _customerRepository.UpdateCustomer(_currentUser.UserId, customer);
                MessageBox.Show("Customer updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId == 0)
                {
                    MessageBox.Show("Please select a customer to delete.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this customer?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _customerRepository.DeleteCustomer(_currentUser.UserId, _selectedCustomerId);
                    MessageBox.Show("Customer deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                var selectedRow = dgvCustomers.SelectedRows[0];
                var customer = selectedRow.DataBoundItem as Customer;

                if (customer != null)
                {
                    _selectedCustomerId = customer.CustomerId;
                    txtCustomerName.Text = customer.CustomerName;
                    txtContactInfo.Text = customer.ContactInfo;
                }
            }
        }
    }
}