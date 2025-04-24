using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;

namespace LaptopStoreApp.Forms
{
    public partial class SupplierForm : UserControl
    {
        private readonly SupplierRepository _supplierRepository; private int _selectedSupplierId = -1;

        public SupplierForm()
        {
            InitializeComponent();
            _supplierRepository = new SupplierRepository();
        }

        private void SupplierForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // Load data into DataGridView
        private void LoadData()
        {
            try
            {
                DataTable suppliersTable = new DataTable();
                var suppliers = _supplierRepository.GetAllSuppliers();

                suppliersTable.Columns.Add("SupplierId", typeof(int));
                suppliersTable.Columns.Add("Name", typeof(string));
                suppliersTable.Columns.Add("Gmail", typeof(string));
                suppliersTable.Columns.Add("Phone", typeof(string));
                suppliersTable.Columns.Add("ImportDate", typeof(DateTime));

                foreach (var supplier in suppliers)
                {
                    suppliersTable.Rows.Add(
                        supplier.SupplierId,
                        supplier.Name,
                        supplier.Gmail,
                        supplier.Phone,
                        DateTime.Now // Placeholder for ImportDate, as it's not in Supplier table
                    );
                }

                dataGridView1.DataSource = suppliersTable;
                dataGridView1.Columns["SupplierId"].HeaderText = "Mã nhà cung cấp";
                dataGridView1.Columns["Name"].HeaderText = "Tên nhà cung cấp";
                dataGridView1.Columns["Gmail"].HeaderText = "Email";
                dataGridView1.Columns["Phone"].HeaderText = "Số điện thoại";
                dataGridView1.Columns["ImportDate"].HeaderText = "Ngày nhập";
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

                _supplierRepository.InsertSupplier(name, gmail, phone);

                MessageBox.Show("Thêm nhà cung cấp thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhà cung cấp: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Sửa" (Update) button click
        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedSupplierId == -1)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp để sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!ValidateInputs()) return;

                string name = textBox1.Text;
                string gmail = textBox3.Text;
                string phone = textBox2.Text;

                _supplierRepository.UpdateSupplier(_selectedSupplierId, name, gmail, phone);

                MessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật nhà cung cấp: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Xóa" (Delete) button click
        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedSupplierId == -1)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa nhà cung cấp này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _supplierRepository.DeleteSupplier(_selectedSupplierId);
                    MessageBox.Show("Xóa nhà cung cấp thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa nhà cung cấp: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handle DataGridView cell click to populate inputs for editing
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                _selectedSupplierId = Convert.ToInt32(row.Cells["SupplierId"].Value);
                textBox1.Text = row.Cells["Name"].Value.ToString();
                textBox3.Text = row.Cells["Gmail"].Value.ToString();
                textBox2.Text = row.Cells["Phone"].Value.ToString();
                if (row.Cells["ImportDate"].Value != DBNull.Value)
                {
                    dateTimePicker1.Value = Convert.ToDateTime(row.Cells["ImportDate"].Value);
                }
            }
        }

        // Validate inputs before Add/Update
        private bool ValidateInputs()
        {
            // Validate Name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            _selectedSupplierId = -1;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dateTimePicker1.Value = DateTime.Now;
        }
    }

}