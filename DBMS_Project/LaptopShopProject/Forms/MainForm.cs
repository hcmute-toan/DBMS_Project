using LaptopShopProject.Forms;
using LaptopShopProject.Models;
using System;
using System.Windows.Forms;

namespace LaptopStoreApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly User _currentUser;

        public MainForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Display username and role
            lblUserName.Text = $"Username: {_currentUser.Username}";
            lbRoleUser.Text = $"Role: {_currentUser.Role}";

            // Enable/disable buttons based on role
            if (_currentUser.Role.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                // Admin has access to all features
                btnUserManagement.Enabled = true;
                btnProductManagement.Enabled = true;
                btnImportManagement.Enabled = true;
                btnExportManagement.Enabled = true;
                btnSupplierManagement.Enabled = true;
                btnCustomerManagement.Enabled = true;
                btnCategoryManagement.Enabled = true;
                btnReport.Enabled = true;
            }
            else if (_currentUser.Role.Equals("employee", StringComparison.OrdinalIgnoreCase))
            {
                // Employee has limited access (e.g., no user or report management)
                btnUserManagement.Enabled = false;
                btnProductManagement.Enabled = true;
                btnImportManagement.Enabled = true;
                btnExportManagement.Enabled = true;
                btnSupplierManagement.Enabled = true;
                btnCustomerManagement.Enabled = true;
                btnCategoryManagement.Enabled = true;
                btnReport.Enabled = false;
            }
        }

        private void btnUserManagement_Click(object sender, EventArgs e)
        {
            // Open User Management Form (to be implemented)
            UserManagementForm userForm = new UserManagementForm(_currentUser);
            userForm.ShowDialog();
        }

        private void btnProductManagement_Click(object sender, EventArgs e)
        {
            // Open Product Management Form (to be implemented)
            ProductManagementForm productForm = new ProductManagementForm(_currentUser);
            productForm.ShowDialog();
        }

        private void btnImportManagement_Click(object sender, EventArgs e)
        {
            // Open Import Management Form (to be implemented)
            ImportManagementForm importForm = new ImportManagementForm(_currentUser);
            importForm.ShowDialog();
        }

        private void btnExportManagement_Click(object sender, EventArgs e)
        {
            // Open Export Management Form (to be implemented)
            ExportManagementForm exportForm = new ExportManagementForm(_currentUser);
            exportForm.ShowDialog();
        }

        private void btnSupplierManagement_Click(object sender, EventArgs e)
        {
            // Open Supplier Management Form (to be implemented)
            SupplierManagementForm supplierForm = new SupplierManagementForm(_currentUser);
            supplierForm.ShowDialog();
        }

        private void btnCustomerManagement_Click(object sender, EventArgs e)
        {
            // Open Customer Management Form (to be implemented)
            CustomerManagementForm customerForm = new CustomerManagementForm(_currentUser);
            customerForm.ShowDialog();
        }

        private void btnCategoryManagement_Click(object sender, EventArgs e)
        {
            // Open Category Management Form (to be implemented)
            CategoryManagementForm categoryForm = new CategoryManagementForm(_currentUser);
            categoryForm.ShowDialog();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            // Open Report Form (to be implemented)
            ReportForm reportForm = new ReportForm(_currentUser);
            reportForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Confirm logout
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Close MainForm and show LoginForm
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }
    }
}