using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private async void UserManagementForm_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
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

        private void InitializeControls()
        {
            cboRole.Items.AddRange(new string[] { "admin_role", "employee_role" });
            cboRole.SelectedIndex = 0;

            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "UserId", HeaderText = "ID", DataPropertyName = "UserId" });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username", DataPropertyName = "Username" });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", HeaderText = "Role", DataPropertyName = "Role" });

            dgvPermisstionLogs.AutoGenerateColumns = false;
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "LogId", HeaderText = "Log ID", DataPropertyName = "LogId" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "Action", HeaderText = "Action", DataPropertyName = "Action" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "ActionDate", HeaderText = "Action Date", DataPropertyName = "ActionDate" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "PerformedBy", HeaderText = "Performed By", DataPropertyName = "PerformedBy" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "TargetRole", HeaderText = "Target Role", DataPropertyName = "TargetRole" });

            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                dgvUsers.DataSource = users;

                var logs = await _userRepository.GetPermissionLogsAsync();
                dgvPermisstionLogs.DataSource = logs;

                ClearForm();
            }
            catch (Exception ex)
            {
                HandleException(ex, "loading data");
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

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string role = cboRole.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(role))
                {
                    MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int newUserId = await _userRepository.InsertUserAsync(txtUserName.Text, txtPassword.Text, role);
                MessageBox.Show($"User added successfully with ID: {newUserId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex, "adding user");
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedUser == null)
                {
                    MessageBox.Show("Please select a user to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUserName.Text))
                {
                    MessageBox.Show("Please enter username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string role = cboRole.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(role))
                {
                    MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await _userRepository.UpdateUserAsync(_selectedUser.UserId, txtUserName.Text, string.IsNullOrEmpty(txtPassword.Text) ? null : txtPassword.Text);

                if (role != _selectedUser.Role)
                {
                    await _userRepository.UpdateUserRoleAsync(_selectedUser.UserId, role);
                }

                MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex, "updating user");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedUser == null)
                {
                    MessageBox.Show("Please select a user to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show($"Are you sure you want to delete user '{_selectedUser.Username}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    await _userRepository.DeleteUserAsync(_selectedUser.UserId);
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "deleting user");
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
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

    public class PermissionLog
    {
        public int LogId { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        public string PerformedBy { get; set; }
        public string TargetRole { get; set; }
    }
}