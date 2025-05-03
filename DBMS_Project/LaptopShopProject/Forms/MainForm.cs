using LaptopShopProject.Forms;
using System;
using System.Windows.Forms;

namespace LaptopStoreApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly string _username;
        private readonly string _role;
        private readonly string _password;
        private Control _currentControl;

        public MainForm(string username, string role, string password)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            _password = password;
            InitializeForm();
        }

        private void InitializeForm()
        {
            lblUserName.Text = $"Username: {_username}";
            lbRoleUser.Text = $"Role: {_role}";

            if (_role.Equals("admin_role", StringComparison.OrdinalIgnoreCase))
            {
                btnUserManagement.Enabled = true;
                btnProductManagement.Enabled = true;
                btnImportManagement.Enabled = true;
                btnExportManagement.Enabled = true;
                btnSupplierManagement.Enabled = true;
                btnCustomerManagement.Enabled = true;
                btnCategoryManagement.Enabled = true;
                btnReport.Enabled = true;
            }
            else if (_role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
            {
                btnUserManagement.Enabled = false;
                btnProductManagement.Enabled = true;
                btnImportManagement.Enabled = true;
                btnExportManagement.Enabled = true;
                btnSupplierManagement.Enabled = true;
                btnCustomerManagement.Enabled = true;
                btnCategoryManagement.Enabled = true;
                btnReport.Enabled = false;
            }
            else
            {
                btnUserManagement.Enabled = false;
                btnProductManagement.Enabled = false;
                btnImportManagement.Enabled = false;
                btnExportManagement.Enabled = false;
                btnSupplierManagement.Enabled = false;
                btnCustomerManagement.Enabled = false;
                btnCategoryManagement.Enabled = false;
                btnReport.Enabled = false;
                MessageBox.Show("Unknown role assigned. Access restricted.", "Role Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            ShowControl(new ProductManagementForm(_username, _role, _password));
        }

        private void ShowControl(Control control)
        {
            if (_currentControl != null)
            {
                pnMainView.Controls.Remove(_currentControl);
                _currentControl.Dispose();
            }

            control.Dock = DockStyle.Fill;
            pnMainView.Controls.Add(control);
            _currentControl = control;
        }

        private void BtnUserManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new UserManagementForm(_username, _role, _password));
        }

        private void BtnProductManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new ProductManagementForm(_username, _role, _password));
        }

        private void BtnImportManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new ImportManagementForm(_username, _role, _password));
        }

        private void BtnExportManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new ExportManagementForm(_username, _role, _password));
        }

        private void BtnSupplierManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new SupplierManagementForm(_username, _role, _password));
        }

        private void BtnCustomerManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new CustomerManagementForm(_username, _role, _password));
        }

        private void BtnCategoryManagement_Click(object sender, EventArgs e)
        {
            ShowControl(new CategoryManagementForm(_username, _role, _password));
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            ShowControl(new ReportForm(_username, _role, _password));
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}