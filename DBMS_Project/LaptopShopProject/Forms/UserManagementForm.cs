using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;

namespace LaptopShopProject.Forms
{
    public partial class UserManagementForm : UserControl
    {
        private readonly UserRepository _userRepository;
        private readonly int _currentUserId;

        // Constructor nhận tham số currentUserId
        public UserManagementForm(int currentUserId)
        {
            InitializeComponent();
            _userRepository = new UserRepository();
            _currentUserId = currentUserId;

            // Đảm bảo chọn toàn bộ hàng
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false; // Chỉ cho phép chọn 1 hàng

            LoadUsers();
            LoadPermissionLogs();
            //LoadRoles();
        }
        private void ClearInputs()
        {
            txtUserName.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = -1;
        }
        private void LoadUsers()
        {
            var users = _userRepository.GetAllUsers(_currentUserId);
            dgvUsers.DataSource = users;
        }

        private void LoadPermissionLogs()
        {
            var logs = _userRepository.GetPermissionLogs(_currentUserId);
            dgvPermisstionLogs.DataSource = logs;
        }

        private void InitializeComboBox()
        {
            cboRole.Items.Clear();
            cboRole.Items.Add("admin");
            cboRole.Items.Add("employee");
            cboRole.SelectedIndex = 0; // Mặc định chọn "admin"
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];
                txtUserName.Text = row.Cells["Username"].Value.ToString();
                txtPassword.Text = ""; // Không hiển thị mật khẩu thực tế
                cboRole.SelectedItem = row.Cells["Role"].Value.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ các ô nhập liệu
            string username = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = cboRole.SelectedItem?.ToString();

            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin (Username, Password, Role)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Gọi InsertUser để thêm người dùng mới
                _userRepository.InsertUser(_currentUserId, username, password, role);

                // Hiển thị thông báo thành công
                MessageBox.Show("Đã thêm người dùng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Làm mới dữ liệu
                LoadUsers();
                LoadPermissionLogs();
                ClearInputs();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi từ cơ sở dữ liệu
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {// Kiểm tra trạng thái chọn
            System.Diagnostics.Debug.WriteLine($"SelectedRows.Count: {dgvUsers.SelectedRows.Count}");

            // Kiểm tra xem có người dùng nào được chọn không
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một người dùng để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lấy user_id của người dùng được chọn
            int selectedUserId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserId"].Value);

            // Xác nhận trước khi xóa
            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa người dùng với ID {selectedUserId}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                // Gọi DeleteUser để xóa người dùng
                _userRepository.DeleteUser(_currentUserId, selectedUserId);

                // Hiển thị thông báo thành công
                MessageBox.Show("Đã xóa người dùng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Làm mới dữ liệu
                LoadUsers();
                LoadPermissionLogs();
                ClearInputs();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}