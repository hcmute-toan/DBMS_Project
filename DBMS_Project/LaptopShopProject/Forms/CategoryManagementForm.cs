using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class CategoryManagementForm : Form
    {
        private readonly CategoryRepository _categoryRepository;
        private int _currentUserId; // Store the ID of the logged-in user
        private int _selectedCategoryId; // Store the ID of the selected category

        public CategoryManagementForm(int currentUserId)
        {
            InitializeComponent();
            _categoryRepository = new CategoryRepository();
            _currentUserId = currentUserId;
            LoadCategories(); // Load categories when the form initializes
        }

        // Load all categories into the DataGridView
        private void LoadCategories()
        {
            try
            {
                List<Category> categories = _categoryRepository.GetAllCategories();
                dgvCategories.DataSource = categories;
                // Optionally, customize column headers
                dgvCategories.Columns["CategoryId"].HeaderText = "ID";
                dgvCategories.Columns["CategoryName"].HeaderText = "Category Name";
                dgvCategories.Columns["Description"].HeaderText = "Description";
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clear input fields and reset selected category
        private void ClearInputs()
        {
            txtCategoryName.Clear();
            txtDescription.Clear();
            _selectedCategoryId = 0;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnAdd.Enabled = true;
        }

        // Handle Add button click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create new category object
                Category newCategory = new Category
                {
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                // Insert category using repository
                _categoryRepository.InsertCategory(_currentUserId, newCategory);
                MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCategories(); // Refresh the grid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle Update button click
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (_selectedCategoryId == 0)
                {
                    MessageBox.Show("Please select a category to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create updated category object
                Category updatedCategory = new Category
                {
                    CategoryId = _selectedCategoryId,
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                // Update category using repository
                _categoryRepository.UpdateCategory(_currentUserId, updatedCategory);
                MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCategories(); // Refresh the grid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle Delete button click
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate selection
                if (_selectedCategoryId == 0)
                {
                    MessageBox.Show("Please select a category to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion
                DialogResult result = MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Delete category using repository
                    _categoryRepository.DeleteCategory(_currentUserId, _selectedCategoryId);
                    MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCategories(); // Refresh the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handle Refresh button click
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategories(); // Reload categories
        }

        // Handle DataGridView row selection
        private void dgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                // Get the selected category
                DataGridViewRow row = dgvCategories.SelectedRows[0];
                _selectedCategoryId = Convert.ToInt32(row.Cells["CategoryId"].Value);
                txtCategoryName.Text = row.Cells["CategoryName"].Value.ToString();
                txtDescription.Text = row.Cells["Description"].Value.ToString();

                // Enable Update and Delete buttons, disable Add button
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnAdd.Enabled = false;
            }
            else
            {
                ClearInputs();
            }
        }
    }
}