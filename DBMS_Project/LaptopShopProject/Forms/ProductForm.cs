using System;
using System.Data;
using System.Windows.Forms;
using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;

namespace LaptopStoreApp.Forms
{
    public partial class ProductForm : UserControl
    {
        private readonly ProductRepository _productRepository; private int _selectedProductId = -1;

        public ProductForm()
        {
            InitializeComponent();
            _productRepository = new ProductRepository();
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // Load data into DataGridView
        private void LoadData()
        {
            try
            {
                DataTable productsTable = new DataTable();
                var products = _productRepository.GetAllProducts();

                productsTable.Columns.Add("ProductId", typeof(int));
                productsTable.Columns.Add("ProductName", typeof(string));
                productsTable.Columns.Add("Description", typeof(string));
                productsTable.Columns.Add("Brand", typeof(string));
                productsTable.Columns.Add("Ean13", typeof(string));
                productsTable.Columns.Add("ImportPrice", typeof(decimal));
                productsTable.Columns.Add("WholesalePrice", typeof(decimal));
                productsTable.Columns.Add("RetailPrice", typeof(decimal));
                productsTable.Columns.Add("ImagePath", typeof(string));
                productsTable.Columns.Add("WarehouseId", typeof(int));
                productsTable.Columns.Add("Status", typeof(bool));

                foreach (var product in products)
                {
                    productsTable.Rows.Add(
                        product.ProductId,
                        product.ProductName,
                        product.Description,
                        product.Brand,
                        product.Ean13,
                        product.ImportPrice,
                        product.WholesalePrice,
                        product.RetailPrice,
                        product.ImagePath,
                        product.WarehouseId,
                        product.Status
                    );
                }

                dataGridView1.DataSource = productsTable;
                dataGridView1.Columns["ProductId"].HeaderText = "Mã sản phẩm";
                dataGridView1.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                dataGridView1.Columns["Description"].HeaderText = "Mô tả";
                dataGridView1.Columns["Brand"].HeaderText = "Thương hiệu";
                dataGridView1.Columns["Ean13"].HeaderText = "Mã EAN13";
                dataGridView1.Columns["ImportPrice"].HeaderText = "Giá nhập";
                dataGridView1.Columns["WholesalePrice"].HeaderText = "Giá sỉ";
                dataGridView1.Columns["RetailPrice"].HeaderText = "Giá bán";
                dataGridView1.Columns["ImagePath"].HeaderText = "Đường dẫn ảnh";
                dataGridView1.Columns["WarehouseId"].HeaderText = "Mã kho";
                dataGridView1.Columns["Status"].HeaderText = "Trạng thái";
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

                string productName = textBox1.Text;
                string description = textBox2.Text;
                string brand = textBox3.Text;
                string ean13 = textBox4.Text;
                decimal importPrice = Convert.ToDecimal(textBox5.Text);
                decimal wholesalePrice = Convert.ToDecimal(textBox6.Text);
                decimal retailPrice = Convert.ToDecimal(textBox7.Text);
                string imagePath = textBox8.Text;
                int warehouseId = Convert.ToInt32(textBox9.Text);
                bool status = checkBox1.Checked;

                _productRepository.InsertProduct(productName, description, brand, ean13, importPrice, wholesalePrice, retailPrice, imagePath, warehouseId, status);

                MessageBox.Show("Thêm sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sản phẩm: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Sửa" (Update) button click
        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedProductId == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!ValidateInputs()) return;

                string productName = textBox1.Text;
                string description = textBox2.Text;
                string brand = textBox3.Text;
                string ean13 = textBox4.Text;
                decimal importPrice = Convert.ToDecimal(textBox5.Text);
                decimal wholesalePrice = Convert.ToDecimal(textBox6.Text);
                decimal retailPrice = Convert.ToDecimal(textBox7.Text);
                string imagePath = textBox8.Text;
                int warehouseId = Convert.ToInt32(textBox9.Text);
                bool status = checkBox1.Checked;

                _productRepository.UpdateProduct(_selectedProductId, productName, description, brand, ean13, importPrice, wholesalePrice, retailPrice, imagePath, warehouseId, status);

                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật sản phẩm: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Xóa" (Delete) button click
        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedProductId == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _productRepository.DeleteProduct(_selectedProductId);
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handle DataGridView cell click to populate inputs for editing
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                _selectedProductId = Convert.ToInt32(row.Cells["ProductId"].Value);
                textBox1.Text = row.Cells["ProductName"].Value.ToString();
                textBox2.Text = row.Cells["Description"].Value.ToString();
                textBox3.Text = row.Cells["Brand"].Value.ToString();
                textBox4.Text = row.Cells["Ean13"].Value.ToString();
                textBox5.Text = row.Cells["ImportPrice"].Value.ToString();
                textBox6.Text = row.Cells["WholesalePrice"].Value.ToString();
                textBox7.Text = row.Cells["RetailPrice"].Value.ToString();
                textBox8.Text = row.Cells["ImagePath"].Value.ToString();
                textBox9.Text = row.Cells["WarehouseId"].Value.ToString();
                checkBox1.Checked = Convert.ToBoolean(row.Cells["Status"].Value);
            }
        }

        // Validate inputs before Add/Update
        private bool ValidateInputs()
        {
            // Validate Product Name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate EAN13 (if provided, must be 13 digits)
            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                string ean13Pattern = @"^\d{13}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(textBox4.Text, ean13Pattern))
                {
                    MessageBox.Show("Mã EAN13 không hợp lệ! Vui lòng nhập 13 chữ số.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // Validate Import Price
            if (string.IsNullOrWhiteSpace(textBox5.Text) || !decimal.TryParse(textBox5.Text, out decimal importPrice) || importPrice <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá nhập hợp lệ (> 0)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate Wholesale Price
            if (string.IsNullOrWhiteSpace(textBox6.Text) || !decimal.TryParse(textBox6.Text, out decimal wholesalePrice) || wholesalePrice <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá sỉ hợp lệ (> 0)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate Retail Price
            if (string.IsNullOrWhiteSpace(textBox7.Text) || !decimal.TryParse(textBox7.Text, out decimal retailPrice) || retailPrice <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá bán hợp lệ (> 0)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate Warehouse ID
            if (string.IsNullOrWhiteSpace(textBox9.Text) || !int.TryParse(textBox9.Text, out int warehouseId) || warehouseId <= 0)
            {
                MessageBox.Show("Vui lòng nhập mã kho hợp lệ (> 0)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Clear input fields
        private void ClearInputs()
        {
            _selectedProductId = -1;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            checkBox1.Checked = false;
        }

        // Handle textBox5 text changed for import price validation
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox5.Text) && !decimal.TryParse(textBox5.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập giá nhập hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Clear();
            }
        }

        // Handle textBox6 text changed for wholesale price validation
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox6.Text) && !decimal.TryParse(textBox6.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập giá sỉ hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox6.Clear();
            }
        }

        // Handle textBox7 text changed for retail price validation
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox7.Text) && !decimal.TryParse(textBox7.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập giá bán hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox7.Clear();
            }
        }

        // Handle textBox9 text changed for warehouse ID validation
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox9.Text) && !int.TryParse(textBox9.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập mã kho hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox9.Clear();
            }
        }
    }

}