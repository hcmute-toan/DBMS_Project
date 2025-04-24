using System;
using System.Data;
using System.Windows.Forms;
using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LaptopStoreApp.Forms
{
    public partial class ImportForm : UserControl
    {
        private readonly ImportRepository _importRepository; private int _selectedImportId = -1;

        public ImportForm()
        {
            InitializeComponent();
            _importRepository = new ImportRepository();
            LoadData();
            LoadComboBoxes();
        }

        // Load data into DataGridView
        private void LoadData()
        {
            try
            {
                dataGridView1.DataSource = _importRepository.GetAllImports();
                dataGridView1.Columns["ImportId"].HeaderText = "Mã phiếu nhập";
                dataGridView1.Columns["ImportDate"].HeaderText = "Ngày nhập";
                dataGridView1.Columns["SupplierId"].Visible = false; // Hide SupplierId
                dataGridView1.Columns["SupplierName"].HeaderText = "Nhà cung cấp";
                dataGridView1.Columns["ProductId"].HeaderText = "Mã sản phẩm";
                dataGridView1.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                dataGridView1.Columns["Quantity"].HeaderText = "Số lượng";
                dataGridView1.Columns["UnitPrice"].HeaderText = "Đơn giá";
                dataGridView1.Columns["TotalAmount"].HeaderText = "Tổng tiền";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load ComboBox data for suppliers and products
        private void LoadComboBoxes()
        {
            try
            {
                // Load suppliers
                DataTable suppliers = _importRepository.GetSuppliers();
                comboBox1.DataSource = suppliers;
                comboBox1.DisplayMember = "name";
                comboBox1.ValueMember = "supplier_id";
                comboBox1.SelectedIndex = -1; // Ensure no default selection

                // Load products
                DataTable products = _importRepository.GetProducts();
                comboBox2.DataSource = products;
                comboBox2.DisplayMember = "product_name";
                comboBox2.ValueMember = "product_id";
                comboBox2.SelectedIndex = -1; // Ensure no default selection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Thêm" (Add) button click
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs()) return;

                DateTime importDate = dateTimePicker1.Value;
                int supplierId = Convert.ToInt32(comboBox1.SelectedValue);
                int productId = Convert.ToInt32(comboBox2.SelectedValue);
                int quantity = Convert.ToInt32(textBox1.Text);
                decimal unitPrice = Convert.ToDecimal(textBox2.Text);

                // Insert import and get the new import ID
                int importId = _importRepository.InsertImport(importDate, supplierId);
                // Insert import detail
                _importRepository.InsertImportDetail(importId, productId, quantity, unitPrice);

                MessageBox.Show("Thêm phiếu nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Sửa" (Update) button click
        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedImportId == -1)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập để sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!ValidateInputs()) return;

                DateTime importDate = dateTimePicker1.Value;
                int supplierId = Convert.ToInt32(comboBox1.SelectedValue);
                int productId = Convert.ToInt32(comboBox2.SelectedValue);
                int quantity = Convert.ToInt32(textBox1.Text);
                decimal unitPrice = Convert.ToDecimal(textBox2.Text);

                _importRepository.UpdateImport(_selectedImportId, importDate, supplierId, productId, quantity, unitPrice);

                MessageBox.Show("Cập nhật phiếu nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Xóa" (Delete) button click
        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedImportId == -1)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa phiếu nhập này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _importRepository.DeleteImport(_selectedImportId);
                    MessageBox.Show("Xóa phiếu nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handle DataGridView cell click to populate inputs for editing
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                _selectedImportId = Convert.ToInt32(row.Cells["ImportId"].Value);
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["ImportDate"].Value);
                comboBox1.SelectedValue = Convert.ToInt32(row.Cells["SupplierId"].Value);
                comboBox2.SelectedValue = Convert.ToInt32(row.Cells["ProductId"].Value);
                textBox1.Text = row.Cells["Quantity"].Value.ToString();
                textBox2.Text = row.Cells["UnitPrice"].Value.ToString();
            }
        }

        // Handle textBox1 text changed for quantity validation
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !int.TryParse(textBox1.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
            }
        }

        // Handle textBox2 text changed for unit price validation
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text) && !decimal.TryParse(textBox2.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập đơn giá hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Clear();
            }
        }

        // Validate inputs before Add/Update
        private bool ValidateInputs()
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(textBox1.Text) || !int.TryParse(textBox1.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ (> 0)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(textBox2.Text) || !decimal.TryParse(textBox2.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Vui lòng nhập đơn giá hợp lệ (> 0)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Clear input fields
        private void ClearInputs()
        {
            _selectedImportId = -1;
            dateTimePicker1.Value = DateTime.Now;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            textBox1.Clear();
            textBox2.Clear();
        }
    }

}