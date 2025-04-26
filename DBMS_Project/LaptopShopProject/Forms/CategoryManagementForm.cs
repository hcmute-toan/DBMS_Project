using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class CategoryManagementForm : UserControl
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly User _currentUser;

        public CategoryManagementForm(User currentUser)
        {
            InitializeComponent();
            _categoryRepository = new CategoryRepository();
            _currentUser = currentUser;
            LoadCategoriesAsync(); // Non-awaited call in constructor
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvCategories.SelectionChanged += dgvCategories_SelectionChanged;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                dgvCategories.DataSource = categories;
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            if (dgvCategories.Columns.Contains("CategoryId"))
                dgvCategories.Columns["CategoryId"].HeaderText = "ID";
            if (dgvCategories.Columns.Contains("CategoryName"))
                dgvCategories.Columns["CategoryName"].HeaderText = "Name";
            if (dgvCategories.Columns.Contains("Description"))
                dgvCategories.Columns["Description"].HeaderText = "Description";
            dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string categoryName = txtCategoryName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Category name is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var category = new Category
                {
                    CategoryName = categoryName,
                    Description = string.IsNullOrEmpty(description) ? null : description
                };
                int categoryId = await _categoryRepository.InsertCategoryAsync(_currentUser.UserId, category);
                MessageBox.Show($"Category added successfully with ID: {categoryId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadCategoriesAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string categoryName = txtCategoryName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Category name is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
                var category = new Category
                {
                    CategoryId = selectedCategory.CategoryId,
                    CategoryName = categoryName,
                    Description = string.IsNullOrEmpty(description) ? null : description
                };
                await _categoryRepository.UpdateCategoryAsync(_currentUser.UserId, category);
                MessageBox.Show("Category updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadCategoriesAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
                    await _categoryRepository.DeleteCategoryAsync(_currentUser.UserId, selectedCategory.CategoryId);
                    MessageBox.Show("Category deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadCategoriesAsync();
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (KeyNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadCategoriesAsync();
        }

        private void dgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
                txtCategoryName.Text = selectedCategory.CategoryName;
                txtDescription.Text = selectedCategory.Description ?? string.Empty;
            }
        }

        private void ClearInputs()
        {
            txtCategoryName.Clear();
            txtDescription.Clear();
            dgvCategories.ClearSelection();
        }
    }
}