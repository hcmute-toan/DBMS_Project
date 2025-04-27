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
        private readonly User _currentUser;
        private readonly UserRepository _userRepository;
        private User _selectedUser; // Lưu người dùng được chọn từ DataGridView

        public UserManagementForm(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _userRepository = new UserRepository();
            InitializeControls();
            // Không gọi LoadDataAsync trực tiếp trong constructor
            this.Load += UserManagementForm_Load; // Gắn sự kiện Load
        }

        private async void UserManagementForm_Load(object sender, EventArgs e)
        {
            await LoadDataAsync(); // Tải dữ liệu bất đồng bộ khi form được load
        }

        private void InitializeControls()
        {
            // Thiết lập ComboBox cho vai trò
            cboRole.Items.AddRange(new string[] { "admin", "employee" });
            cboRole.SelectedIndex = 0; // Mặc định chọn "admin"

            // Thiết lập DataGridView cho người dùng
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "UserId", HeaderText = "ID", DataPropertyName = "UserId" });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username", DataPropertyName = "Username" });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", HeaderText = "Role", DataPropertyName = "Role" });

            // Thiết lập DataGridView cho nhật ký quyền
            dgvPermisstionLogs.AutoGenerateColumns = false;
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "LogId", HeaderText = "Log ID", DataPropertyName = "LogId" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username", DataPropertyName = "Username" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "Action", HeaderText = "Action", DataPropertyName = "Action" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "OldRole", HeaderText = "Old Role", DataPropertyName = "OldRole" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "NewRole", HeaderText = "New Role", DataPropertyName = "NewRole" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "ActionDate", HeaderText = "Action Date", DataPropertyName = "ActionDate" });
            dgvPermisstionLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "PerformedByUsername", HeaderText = "Performed By", DataPropertyName = "PerformedByUsername" });

            // Gắn sự kiện chọn dòng trên DataGridView
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Tải danh sách người dùng
                var users = await _userRepository.GetAllUsersAsync(_currentUser.UserId);
                dgvUsers.DataSource = users;

                // Tải nhật ký quyền
                var logs = await _userRepository.GetPermissionLogsAsync(_currentUser.UserId);
                dgvPermisstionLogs.DataSource = logs;

                // Xóa lựa chọn và làm sạch form
                ClearForm();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    txtPassword.Clear(); // Không hiển thị mật khẩu để bảo mật
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

                int newUserId = await _userRepository.InsertUserAsync(_currentUser.UserId, txtUserName.Text, txtPassword.Text, role);
                MessageBox.Show($"User added successfully with ID: {newUserId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // Cập nhật thông tin người dùng
                await _userRepository.UpdateUserAsync(_currentUser.UserId, _selectedUser.UserId, txtUserName.Text, string.IsNullOrEmpty(txtPassword.Text) ? null : txtPassword.Text);

                // Cập nhật vai trò nếu thay đổi
                if (role != _selectedUser.Role)
                {
                    await _userRepository.UpdateUserRoleAsync(_currentUser.UserId, _selectedUser.UserId, role);
                }

                MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    await _userRepository.DeleteUserAsync(_currentUser.UserId, _selectedUser.UserId);
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataAsync().GetAwaiter().GetResult(); // Gọi đồng bộ trong sự kiện đồng bộ
        }
    }

    // Định nghĩa lớp PermissionLog để sử dụng trong GetPermissionLogs
    public class PermissionLog
    {
        public int LogId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string OldRole { get; set; }
        public string NewRole { get; set; }
        public DateTime ActionDate { get; set; }
        public string PerformedByUsername { get; set; }
    }
}