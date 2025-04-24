using System;
using System.Data;
using System.Windows.Forms;
using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;

namespace LaptopStoreApp.Forms
{
    public partial class CategoryForm : UserControl
    {
        private readonly CategoryRepository _categoryRepository; private int _selectedCategoryId = -1;

        public CategoryForm()
        {
            InitializeComponent();
            _categoryRepository = new CategoryRepository();
        }

        private void CategoryForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // Load data into DataGridView
        private void LoadData()
        {
            try
            {
                DataTable categoriesTable = new DataTable();
                var categories = _categoryRepository.GetAllCategories();

                categoriesTable.Columns.Add("CategoryId", typeof(int));
                categoriesTable.Columns.Add("CategoryName", typeof(string));
                categoriesTable.Columns.Add("Description", typeof(string));

                foreach (var category in categories)
                {
                    categoriesTable.Rows.Add(
                        category.CategoryId,
                        category.CategoryName,
                        category.Description
                    );
                }

                dataGridView1.DataSource = categoriesTable;
                dataGridView1.Columns["CategoryId"].HeaderText = "Mã danh mục";
                dataGridView1.Columns["CategoryName"].HeaderText = "Tên danh mục";
                dataGridView1.Columns["Description"].HeaderText = "Mô tả";
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

                string categoryName = textBox1.Text;
                string description = textBox2.Text;

                _categoryRepository.InsertCategory(categoryName, description);

                MessageBox.Show("Thêm danh mục thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm danh mục: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Sửa" (Update) button click
        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedCategoryId == -1)
            {
                MessageBox.Show("Vui lòng chọn danh mục để sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!ValidateInputs()) return;

                string categoryName = textBox1.Text;
                string description = textBox2.Text;

                _categoryRepository.UpdateCategory(_selectedCategoryId, categoryName, description);

                MessageBox.Show("Cập nhật danh mục thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật danh mục: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle "Xóa" (Delete) button click
        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedCategoryId == -1)
            {
                MessageBox.Show("Vui lòng chọn danh mục để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa danh mục này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _categoryRepository.DeleteCategory(_selectedCategoryId);
                    MessageBox.Show("Xóa danh mục thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa danh mục: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handle DataGridView cell click to populate inputs for editing
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                _selectedCategoryId = Convert.ToInt32(row.Cells["CategoryId"].Value);
                textBox1.Text = row.Cells["CategoryName"].Value.ToString();
                textBox2.Text = row.Cells["Description"].Value.ToString();
            }
        }

        // Validate inputs before Add/Update
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Clear input fields
        private void ClearInputs()
        {
            _selectedCategoryId = -1;
            textBox1.Clear();
            textBox2.Clear();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Empty event handler, can be removed if not needed
        }
    }

}