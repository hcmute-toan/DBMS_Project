using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;

namespace LaptopStoreApp.Forms
{
    public partial class CustomerForm : UserControl
    {
        private readonly CustomerRepository _customerRepository; private int _selectedCustomerId = -1;

        public CustomerForm()
        {
            InitializeComponent();
            _customerRepository = new CustomerRepository();
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // Load data into DataGridView
        private void LoadData()
        {
            try
            {
                DataTable customersTable = new DataTable();
                var customers = _customerRepository.GetAllCustomers();

                customersTable.Columns.Add("CustomerId", typeof(int));
                customersTable.Columns.Add("Name", typeof(string));
                customersTable.Columns.Add("Gmail", typeof(string));
                customersTable.Columns.Add("Phone", typeof(string));
                customersTable.Columns.Add("Address", typeof(string));
                customersTable.Columns.Add("ExportDate", typeof(DateTime));

                foreach (var customer in customers)
                {
                    customersTable.Rows.Add(
                        customer.CustomerId,
                        customer.Name,
                        customer.Gmail,
                        customer.Phone,
                        customer.Address ?? "", // Placeholder for Address (not in DB)
                        DateTime.Now // Placeholder for ExportDate (not in Customer table)
                    );
                }

                dataGridView1.DataSource = customersTable;
                dataGridView1.Columns["CustomerId"].HeaderText = "Mã khách hàng";
                dataGridView1.Columns["Name"].HeaderText = "Tên khách hàng";
                dataGridView1.Columns["Gmail"].HeaderText = "Email";
                dataGridView1.Columns["Phone"].HeaderText = "Số điện thoại";
                dataGridView1.Columns["Address"].HeaderText = "Địa chỉ";
                dataGridView1.Columns["ExportDate"].HeaderText = "Ngày nhập";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Thêm" (Add) button click
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs()) return;

                string name = textBox1.Text;
                string gmail = textBox3.Text;
                string phone = textBox2.Text;

                _customerRepository.InsertCustomer(name, gmail, phone);

                MessageBox.Show("Thêm khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Sửa" (Update) button click
        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId == -1)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!ValidateInputs()) return;

                string name = textBox1.Text;
                string gmail = textBox3.Text;
                string phone = textBox2.Text;

                _customerRepository.UpdateCustomer(_selectedCustomerId, name, gmail, phone);

                MessageBox.Show("Cập nhật khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật khách hàng: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Xóa" (Delete) button click
        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId == -1)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _customerRepository.DeleteCustomer(_selectedCustomerId);
                    MessageBox.Show("Xóa khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handle DataGridView cell click to populate inputs for editing
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                _selectedCustomerId = Convert.ToInt32(row.Cells["CustomerId"].Value);
                textBox1.Text = row.Cells["Name"].Value.ToString();
                textBox3.Text = row.Cells["Gmail"].Value.ToString();
                textBox2.Text = row.Cells["Phone"].Value.ToString();
                textBox4.Text = row.Cells["Address"].Value.ToString();
                if (row.Cells["ExportDate"].Value != DBNull.Value)
                {
                    dateTimePicker1.Value = Convert.ToDateTime(row.Cells["ExportDate"].Value);
                }
            }
        }

        // Validate inputs before Add/Update
        private bool ValidateInputs()
        {
            // Validate Name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate Gmail
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Vui lòng nhập email!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(textBox3.Text, emailPattern))
            {
                MessageBox.Show("Email không hợp lệ! Vui lòng nhập email đúng định dạng (ví dụ: example@domain.com).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate Phone
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string phonePattern = @"^\d{10}$";
            if (!Regex.IsMatch(textBox2.Text, phonePattern))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Vui lòng nhập số điện thoại gồm 10 chữ số.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Clear input fields
        private void ClearInputs()
        {
            _selectedCustomerId = -1;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            dateTimePicker1.Value = DateTime.Now;
        }
    }

}