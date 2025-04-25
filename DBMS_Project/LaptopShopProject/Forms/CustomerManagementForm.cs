using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class CustomerManagementForm : Form
    {
        private readonly CustomerRepository _customerRepository;
        private int _currentUserId; // Store the ID of the logged-in user
        private int _selectedCustomerId; // Store the ID of the selected customer

        public CustomerManagementForm(int currentUserId)
        {
            InitializeComponent();
            _customerRepository = new CustomerRepository();
            _currentUserId = currentUserId;
            LoadCustomers(); // Load customers when the form initializes
        }

        // Load all customers into the DataGridView
        private void LoadCustomers()
        {
            try
            {
                List<Customer> customers = _customerRepository.GetAllCustomers(_currentUserId);
                dgvCustomers.DataSource = customers;
                // Customize column headers
                dgvCustomers.Columns["CustomerId"].HeaderText = "ID";
                dgvCustomers.Columns["CustomerName"].HeaderText = "Customer Name";
                dgvCustomers.Columns["ContactInfo"].HeaderText = "Contact Info";
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clear input fields and reset selected customer
        private void ClearInputs()
        {
            txtCustomerName.Clear();
            txtContactInfo.Clear();
            _selectedCustomerId = 0;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnAdd.Enabled = true;
        }

        // Handle Add button click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("Please enter a customer name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtContactInfo.Text))
                {
                    MessageBox.Show("Please enter contact information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create new customer object
                Customer newCustomer = new Customer
                {
                    CustomerName = txtCustomerName.Text.Trim(),
                    ContactInfo = txtContactInfo.Text.Trim()
                };

                // Insert customer using repository
                _customerRepository.InsertCustomer(_currentUserId, newCustomer);
                MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCustomers(); // Refresh the grid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle Update button click
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (_selectedCustomerId == 0)
                {
                    MessageBox.Show("Please select a customer to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("Please enter a customer name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtContactInfo.Text))
                {
                    MessageBox.Show("Please enter contact information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create updated customer object
                Customer updatedCustomer = new Customer
                {
                    CustomerId = _selectedCustomerId,
                    CustomerName = txtCustomerName.Text.Trim(),
                    ContactInfo = txtContactInfo.Text.Trim()
                };

                // Update customer using repository
                _customerRepository.UpdateCustomer(_currentUserId, updatedCustomer);
                MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCustomers(); // Refresh the grid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle Delete button click
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate selection
                if (_selectedCustomerId == 0)
                {
                    MessageBox.Show("Please select a customer to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion
                DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Delete customer using repository
                    _customerRepository.DeleteCustomer(_currentUserId, _selectedCustomerId);
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers(); // Refresh the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle Refresh button click
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers(); // Reload customers
        }

        // Handle DataGridView row selection
        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                // Get the selected customer
                DataGridViewRow row = dgvCustomers.SelectedRows[0];
                _selectedCustomerId = Convert.ToInt32(row.Cells["CustomerId"].Value);
                txtCustomerName.Text = row.Cells["CustomerName"].Value.ToString();
                txtContactInfo.Text = row.Cells["ContactInfo"].Value.ToString();

                // Enable Update and Delete buttons, disable Add button
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnAdd.Enabled = false;
            }
            else
            {
                ClearInputs();
            }
        }
    }
}