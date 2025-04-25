using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class CategoryManagementForm : UserControl
    {
        private readonly User _currentUser;
        private readonly CategoryRepository _categoryRepository;
        private int _selectedCategoryId = 0;

        public CategoryManagementForm(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _categoryRepository = new CategoryRepository();
            // Gắn sự kiện SelectionChanged để đảm bảo chọn dòng hiển thị dữ liệu
            dgvCategories.SelectionChanged += dgvCategories_SelectionChanged;
            // Gắn sự kiện Click cho các nút
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _categoryRepository.GetAllCategories();
                if (categories == null || !categories.Any())
                {
                    MessageBox.Show("No categories found.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvCategories.DataSource = null;
                    ClearInputs();
                    return;
                }

                dgvCategories.DataSource = categories;

                // Configure DataGridView columns
                if (dgvCategories.Columns.Contains("CategoryId"))
                    dgvCategories.Columns["CategoryId"].HeaderText = "ID";
                if (dgvCategories.Columns.Contains("CategoryName"))
                    dgvCategories.Columns["CategoryName"].HeaderText = "Category Name";
                if (dgvCategories.Columns.Contains("Description"))
                    dgvCategories.Columns["Description"].HeaderText = "Description";

                // Clear selection
                dgvCategories.ClearSelection();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtCategoryName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            _selectedCategoryId = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    MessageBox.Show("Category name is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show("Description is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var category = new Category
                {
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                _categoryRepository.InsertCategory(_currentUser.UserId, category);
                MessageBox.Show("Category added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCategoryId == 0 || dgvCategories.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a category to update.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    MessageBox.Show("Category name is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show("Description is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var category = new Category
                {
                    CategoryId = _selectedCategoryId,
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                _categoryRepository.UpdateCategory(_currentUser.UserId, category);
                MessageBox.Show("Category updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCategoryId == 0 || dgvCategories.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a category to delete.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this category?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _categoryRepository.DeleteCategory(_currentUser.UserId, _selectedCategoryId);
                    MessageBox.Show("Category deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadCategories();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing categories: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvCategories.SelectedRows.Count > 0)
                {
                    var selectedRow = dgvCategories.SelectedRows[0];
                    var category = selectedRow.DataBoundItem as Category;

                    if (category != null)
                    {
                        _selectedCategoryId = category.CategoryId;
                        txtCategoryName.Text = category.CategoryName ?? string.Empty;
                        txtDescription.Text = category.Description ?? string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Selected row does not contain valid category data.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClearInputs();
                    }
                }
                else
                {
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearInputs();
            }
        }
    }
}