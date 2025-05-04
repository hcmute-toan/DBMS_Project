using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class UserManagementForm : UserControl
    {
        private readonly string _username;
        private readonly string _role;
        private readonly string _password;
        private readonly UserRepository _userRepository;
        private User _selectedUser;

        public UserManagementForm(string username, string role, string password)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            _password = password;
            _userRepository = new UserRepository(_username, _password);
            InitializeControls();
            ConfigurePermissions();
            this.Load += UserManagementForm_Load;
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            LoadDataAsync();
        }

        private void ConfigurePermissions()
        {
            // Vô hiệu hóa tất cả các nút vì không có chức năng quản lý người dùng trong cơ sở dữ liệu hiện tại
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void InitializeControls()
        {
            cboRole.Items.AddRange(new string[] { "admin_role", "employee_role" });
            cboRole.SelectedIndex = 0;

            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "UserId", HeaderText = "ID", DataPropertyName = "UserId" });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username", DataPropertyName = "Username" });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", HeaderText = "Role", DataPropertyName = "Role" });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreateDate", HeaderText = "Create Date", DataPropertyName = "CreateDate" });

            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
        }

        private void LoadDataAsync()
        {
            try
            {
                var users = _userRepository.GetAllUsers();
                dgvUsers.DataSource = users;
            }
            catch (Exception ex)
            {
                HandleException(ex, "loading user data");
            }
        }

        private void ClearForm()
        {
            txtUserName.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = 0;
            _selectedUser = null;
            dgvUsers.ClearSelection();
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                _selectedUser = dgvUsers.SelectedRows[0].DataBoundItem as User;
                if (_selectedUser != null)
                {
                    txtUserName.Text = _selectedUser.Username;
                    cboRole.SelectedItem = _selectedUser.Role;
                    txtPassword.Clear();
                }
            }
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