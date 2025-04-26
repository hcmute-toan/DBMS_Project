using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class UserManagementForm : UserControl
    {
        private readonly UserRepository _userRepository;
        private readonly User _currentUser;

        public UserManagementForm(User currentUser)
        {
            InitializeComponent();
            //_userRepository = new UserRepository();
            //_currentUser = currentUser;
            //ConfigureRoleComboBox();
            //LoadUsersAsync(); // Non-awaited in constructor
            //LoadPermissionLogsAsync(); // Non-awaited in constructor
            //ConfigureEventHandlers();
        }

        //private void ConfigureEventHandlers()
        //{
        //    btnAdd.Click += btnAdd_Click;
        //    btnUpdate.Click += btnUpdate_Click;
        //    btnDelete.Click += btnDelete_Click;
        //    btnRefresh.Click += btnRefresh_Click;
        //    dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;
        //}

        //private void ConfigureRoleComboBox()
        //{
        //    cboRole.Items.AddRange(new[] { "admin", "employee" });
        //    cboRole.SelectedIndex = 0;
        //}

        //private async Task LoadUsersAsync()
        //{
        //    try
        //    {
        //        var users = await _userRepository.GetAllUsersAsync(_currentUser.UserId);
        //        dgvUsers.DataSource = users;
        //        ConfigureUserDataGridView();
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private async Task LoadPermissionLogsAsync()
        //{
        //    try
        //    {
        //        var logs = await _userRepository.GetPermissionLogsAsync(_currentUser.UserId);
        //        dgvPermissionLogs.DataSource = logs;
        //        ConfigurePermissionLogsDataGridView();
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error loading permission logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void ConfigureUserDataGridView()
        //{
        //    if (dgvUsers.Columns.Contains("UserId"))
        //        dgvUsers.Columns["UserId"].HeaderText = "ID";
        //    if (dgvUsers.Columns.Contains("Username"))
        //        dgvUsers.Columns["Username"].HeaderText = "Username";
        //    if (dgvUsers.Columns.Contains("Role"))
        //        dgvUsers.Columns["Role"].HeaderText = "Role";
        //    dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //}

        //private void ConfigurePermissionLogsDataGridView()
        //{
        //    if (dgvPermissionLogs.Columns.Contains("LogId"))
        //        dgvPermissionLogs.Columns["LogId"].HeaderText = "Log ID";
        //    if (dgvPermissionLogs.Columns.Contains("UserId"))
        //        dgvPermissionLogs.Columns["UserId"].Visible = false;
        //    if (dgvPermissionLogs.Columns.Contains("Username"))
        //        dgvPermissionLogs.Columns["Username"].HeaderText = "Username";
        //    if (dgvPermissionLogs.Columns.Contains("Action"))
        //        dgvPermissionLogs.Columns["Action"].HeaderText = "Action";
        //    if (dgvPermissionLogs.Columns.Contains("OldRole"))
        //        dgvPermissionLogs.Columns["OldRole"].HeaderText = "Old Role";
        //    if (dgvPermissionLogs.Columns.Contains("NewRole"))
        //        dgvPermissionLogs.Columns["NewRole"].HeaderText = "New Role";
        //    if (dgvPermissionLogs.Columns.Contains("ActionDate"))
        //        dgvPermissionLogs.Columns["ActionDate"].HeaderText = "Date";
        //    if (dgvPermissionLogs.Columns.Contains("PerformedBy"))
        //        dgvPermissionLogs.Columns["PerformedBy"].Visible = false;
        //    if (dgvPermissionLogs.Columns.Contains("PerformedByUsername"))
        //        dgvPermissionLogs.Columns["PerformedByUsername"].HeaderText = "Performed By";
        //    dgvPermissionLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //}

        //private async void btnAdd_Click(object sender, EventArgs e)
        //{
        //    string username = txtUserName.Text.Trim();
        //    string password = txtPassword.Text.Trim();
        //    string role = cboRole.SelectedItem?.ToString();

        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
        //    {
        //        MessageBox.Show("Username, password, and role are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (password.Length < 6)
        //    {
        //        MessageBox.Show("Password must be at least 6 characters long.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    try
        //    {
        //        int userId = await _userRepository.InsertUserAsync(_currentUser.UserId, username, password, role);
        //        MessageBox.Show($"User added successfully with ID: {userId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        ClearInputs();
        //        await LoadUsersAsync();
        //        await LoadPermissionLogsAsync();
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private async void btnUpdate_Click(object sender, EventArgs e)
        //{
        //    if (dgvUsers.SelectedRows.Count == 0)
        //    {
        //        MessageBox.Show("Please select a user to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    string username = txtUserName.Text.Trim();
        //    string password = txtPassword.Text.Trim();
        //    string role = cboRole.SelectedItem?.ToString();

        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
        //    {
        //        MessageBox.Show("Username and role are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (!string.IsNullOrEmpty(password) && password.Length < 6)
        //    {
        //        MessageBox.Show("Password must be at least 6 characters long if provided.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    try
        //    {
        //        var selectedUser = (User)dgvUsers.SelectedRows[0].DataBoundItem;
        //        // Update username and password (password is optional)
        //        await _userRepository.UpdateUserAsync(_currentUser.UserId, selectedUser.UserId, username, string.IsNullOrEmpty(password) ? null : password);
        //        // Update role if changed
        //        if (selectedUser.Role != role)
        //        {
        //            await _userRepository.UpdateUserRoleAsync(_currentUser.UserId, selectedUser.UserId, role);
        //        }
        //        MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        ClearInputs();
        //        await LoadUsersAsync();
        //        await LoadPermissionLogsAsync();
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private async void btnDelete_Click(object sender, EventArgs e)
        //{
        //    if (dgvUsers.SelectedRows.Count == 0)
        //    {
        //        MessageBox.Show("Please select a user to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    var selectedUser = (User)dgvUsers.SelectedRows[0].DataBoundItem;
        //    if (selectedUser.UserId == _currentUser.UserId)
        //    {
        //        MessageBox.Show("You cannot delete your own account.", "Operation Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (MessageBox.Show($"Are you sure you want to delete the user '{selectedUser.Username}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //    {
        //        try
        //        {
        //            await _userRepository.DeleteUserAsync(_currentUser.UserId, selectedUser.UserId);
        //            MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            ClearInputs();
        //            await LoadUsersAsync();
        //            await LoadPermissionLogsAsync();
        //        }
        //        catch (InvalidOperationException ex)
        //        {
        //            MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //        catch (UnauthorizedAccessException ex)
        //        {
        //            MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //        catch (KeyNotFoundException ex)
        //        {
        //            MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        //private async void btnRefresh_Click(object sender, EventArgs e)
        //{
        //    ClearInputs();
        //    await LoadUsersAsync();
        //    await LoadPermissionLogsAsync();
        //}

        //private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        //{
        //    if (dgvUsers.SelectedRows.Count > 0)
        //    {
        //        var selectedUser = (User)dgvUsers.SelectedRows[0].DataBoundItem;
        //        txtUserName.Text = selectedUser.Username;
        //        txtPassword.Clear(); // Password not retrieved for security
        //        cboRole.SelectedItem = selectedUser.Role;
        //    }
        //}

        //private void ClearInputs()
        //{
        //    txtUserName.Clear();
        //    txtPassword.Clear();
        //    cboRole.SelectedIndex = 0;
        //    dgvUsers.ClearSelection();
        //    dgvPermissionLogs.ClearSelection();
        //}
    }
}