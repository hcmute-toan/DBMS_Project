using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;

namespace LaptopStoreApp.Forms
{
    public partial class ExportForm : UserControl
    {
        private readonly ExportRepository _exportRepository; private int _selectedExportId = -1; private readonly string connectionString = "Data Source=YNWA\\SQLEXPRESS;Initial Catalog=DBMS_2;Integrated Security=True";

        public ExportForm()
        {
            InitializeComponent();
            _exportRepository = new ExportRepository();
            LoadData();
            LoadComboBoxes();
        }

        // Load data into DataGridView
        private void LoadData()
        {
            try
            {
                DataTable exportsTable = new DataTable();
                var exports = _exportRepository.GetAllExports();

                exportsTable.Columns.Add("ExportId", typeof(int));
                exportsTable.Columns.Add("ExportDate", typeof(DateTime));
                exportsTable.Columns.Add("CustomerName", typeof(string));
                exportsTable.Columns.Add("ProductId", typeof(int));
                exportsTable.Columns.Add("ProductName", typeof(string));
                exportsTable.Columns.Add("Quantity", typeof(int));
                exportsTable.Columns.Add("UnitPrice", typeof(decimal));
                exportsTable.Columns.Add("TotalAmount", typeof(decimal));

                foreach (var export in exports)
                {
                    exportsTable.Rows.Add(
                        export.ExportId,
                        export.ExportDate,
                        export.CustomerName,
                        export.ProductId,
                        export.ProductName,
                        export.Quantity,
                        export.UnitPrice,
                        export.TotalAmount
                    );
                }

                dataGridView1.DataSource = exportsTable;
                dataGridView1.Columns["ExportId"].HeaderText = "Mã phiếu xuất";
                dataGridView1.Columns["ExportDate"].HeaderText = "Ngày xuất";
                dataGridView1.Columns["CustomerName"].HeaderText = "Khách hàng";
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

        // Load ComboBox data for customers and products
        private void LoadComboBoxes()
        {
            try
            {
                // Load customers
                DataTable customers = _exportRepository.GetCustomers();
                comboBox1.DataSource = customers;
                comboBox1.DisplayMember = "name";
                comboBox1.ValueMember = "customer_id";

                // Load products (comboBox2 is for products, not warehouse)
                DataTable products = _exportRepository.GetProducts();
                comboBox2.DataSource = products;
                comboBox2.DisplayMember = "product_name";
                comboBox2.ValueMember = "product_id";
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

                DateTime exportDate = dateTimePicker1.Value;
                int customerId = Convert.ToInt32(comboBox1.SelectedValue);
                int productId = Convert.ToInt32(comboBox2.SelectedValue);
                int quantity = Convert.ToInt32(textBox1.Text);
                decimal unitPrice = GetUnitPrice(productId);

                int exportId = _exportRepository.InsertExport(exportDate, customerId);
                _exportRepository.InsertExportDetail(exportId, productId, quantity, unitPrice);

                MessageBox.Show("Thêm phiếu xuất thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phiếu xuất: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Sửa" (Update) button click
        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedExportId == -1)
            {
                MessageBox.Show("Vui lòng chọn phiếu xuất để sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!ValidateInputs()) return;

                DateTime exportDate = dateTimePicker1.Value;
                int customerId = Convert.ToInt32(comboBox1.SelectedValue);
                int productId = Convert.ToInt32(comboBox2.SelectedValue);
                int quantity = Convert.ToInt32(textBox1.Text);
                decimal unitPrice = GetUnitPrice(productId);

                // Update the export
                _exportRepository.UpdateExport(_selectedExportId, exportDate, customerId);

                // Delete existing export detail
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM ExportDetail WHERE export_id = @export_id", conn);
                    cmd.Parameters.AddWithValue("@export_id", _selectedExportId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Insert new export detail
                _exportRepository.InsertExportDetail(_selectedExportId, productId, quantity, unitPrice);

                MessageBox.Show("Cập nhật phiếu xuất thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật phiếu xuất: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Xóa" (Delete) button click
        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedExportId == -1)
            {
                MessageBox.Show("Vui lòng chọn phiếu xuất để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa phiếu xuất này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _exportRepository.DeleteExport(_selectedExportId);
                    MessageBox.Show("Xóa phiếu xuất thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa phiếu xuất: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handle DataGridView cell click to populate inputs for editing
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                _selectedExportId = Convert.ToInt32(row.Cells["ExportId"].Value);
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["ExportDate"].Value);
                comboBox1.Text = row.Cells["CustomerName"].Value.ToString();
                comboBox2.Text = row.Cells["ProductName"].Value.ToString();
                textBox1.Text = row.Cells["Quantity"].Value.ToString();
            }
        }

        // Handle textBox1 text changed for validation
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !int.TryParse(textBox1.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
            }
        }

        // Validate inputs before Add/Update
        private bool ValidateInputs()
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            return true;
        }

        // Clear input fields
        private void ClearInputs()
        {
            _selectedExportId = -1;
            dateTimePicker1.Value = DateTime.Now;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            textBox1.Clear();
        }

        // Helper method to get unit price from product
        private decimal GetUnitPrice(int productId)
        {
            DataTable products = _exportRepository.GetProducts();
            DataRow product = products.AsEnumerable().FirstOrDefault(p => p.Field<int>("product_id") == productId);
            return product != null ? Convert.ToDecimal(product["sell_price"]) : 0;
        }
    }

}