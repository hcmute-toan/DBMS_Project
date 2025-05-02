using LaptopShopProject.Forms;
using LaptopShopProject.Models;
using System;
using System.Windows.Forms;

namespace LaptopStoreApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly User _currentUser;
        private Control _currentControl; // To keep track of the currently displayed control

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
                //btnUserManagement.Enabled = false;
                btnUserManagement.Enabled = true;
                btnProductManagement.Enabled = true;
                btnImportManagement.Enabled = true;
                btnExportManagement.Enabled = true;
                btnSupplierManagement.Enabled = true;
                btnCustomerManagement.Enabled = true;
                btnCategoryManagement.Enabled = true;
                btnReport.Enabled = false;
            }
            ShowControl(new UserManagementForm(_currentUser));
        }

        private void ShowControl(Control control)
        {
            // Clear the current control from the panel
            if (_currentControl != null)
            {
                pnMainView.Controls.Remove(_currentControl);
                _currentControl.Dispose();
            }

            // Add the new control to the panel
            control.Dock = DockStyle.Fill;
            pnMainView.Controls.Add(control);
            _currentControl = control;
        }

        private void BtnUserManagement_Click(object sender, EventArgs e)
        {
            // Placeholder for UserManagementForm (as UserControl)
            // ShowControl(new UserManagementForm(_currentUser));
            ShowControl(new UserManagementForm(_currentUser));
        }

        private void BtnProductManagement_Click(object sender, EventArgs e)
        {
            // Placeholder for ProductManagementForm (as UserControl)
            // ShowControl(new ProductManagementForm(_currentUser));
            ShowControl(new ProductManagementForm(_currentUser));
        }

        private void BtnImportManagement_Click(object sender, EventArgs e)
        {
            // Placeholder for ImportManagementForm (as UserControl)
            // ShowControl(new ImportManagementForm(_currentUser));
            ShowControl(new ImportManagementForm(_currentUser));
        }

        private void BtnExportManagement_Click(object sender, EventArgs e)
        {
            // Placeholder for ExportManagementForm (as UserControl)
            // ShowControl(new ExportManagementForm(_currentUser));
            ShowControl(new ExportManagementForm(_currentUser));
        }

        private void BtnSupplierManagement_Click(object sender, EventArgs e)
        {
            // Placeholder for SupplierManagementForm (as UserControl)
            // ShowControl(new SupplierManagementForm(_currentUser));
            ShowControl(new SupplierManagementForm(_currentUser));
        }

        private void BtnCustomerManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new CustomerManagementForm(_currentUser));
        }

        private void BtnCategoryManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new CategoryManagementForm(_currentUser));
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            // Placeholder for ReportForm (as UserControl)
            // ShowControl(new ReportForm(_currentUser));
            ShowControl(new ReportForm(_currentUser));
        }

        private void BtnLogout_Click(object sender, EventArgs e)
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit(); // Ensure the application exits completely
        }
    }
}