using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class ProductManagementForm : UserControl
    {
        private readonly ProductRepository _productRepository;
        private readonly User _currentUser;

        public ProductManagementForm(User currentUser)
        {
            InitializeComponent();
            _productRepository = new ProductRepository();
            _currentUser = currentUser;
            LoadProductsAsync(); // Non-awaited in constructor
            LoadProductLogsAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync(_currentUser.UserId);
                dgvProducts.DataSource = products;
                ConfigureProductDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductLogsAsync()
        {
            try
            {
                var logs = await _productRepository.GetProductLogsAsync();
                dgvProductLogs.DataSource = logs;
                ConfigureProductLogsDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureProductDataGridView()
        {
            if (dgvProducts.Columns.Contains("ProductId"))
                dgvProducts.Columns["ProductId"].HeaderText = "ID";
            if (dgvProducts.Columns.Contains("ProductName"))
                dgvProducts.Columns["ProductName"].HeaderText = "Name";
            if (dgvProducts.Columns.Contains("Price"))
                dgvProducts.Columns["Price"].HeaderText = "Price";
            if (dgvProducts.Columns.Contains("StockQuantity"))
                dgvProducts.Columns["StockQuantity"].HeaderText = "Stock Quantity";
            if (dgvProducts.Columns.Contains("Brands"))
                dgvProducts.Columns["Brands"].HeaderText = "Brands";
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureProductLogsDataGridView()
        {
            if (dgvProductLogs.Columns.Contains("LogId"))
                dgvProductLogs.Columns["LogId"].HeaderText = "Log ID";
            if (dgvProductLogs.Columns.Contains("ProductId"))
                dgvProductLogs.Columns["ProductId"].HeaderText = "Product ID";
            if (dgvProductLogs.Columns.Contains("ProductName"))
                dgvProductLogs.Columns["ProductName"].HeaderText = "Product Name";
            if (dgvProductLogs.Columns.Contains("DeletedDate"))
                dgvProductLogs.Columns["DeletedDate"].HeaderText = "Deleted Date";
            if (dgvProductLogs.Columns.Contains("DeletedBy"))
                dgvProductLogs.Columns["DeletedBy"].HeaderText = "Deleted By";
            dgvProductLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateProductInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var product = new Product
                {
                    ProductName = txtProductName.Text.Trim(),
                    Price = decimal.Parse(txtPrice.Text.Trim()),
                    StockQuantity = int.Parse(txtStockQuantity.Text.Trim())
                };
                int productId = await _productRepository.InsertProductAsync(_currentUser.UserId, product);
                MessageBox.Show($"Product added successfully with ID: {productId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadProductsAsync();
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
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateProductInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                var product = new Product
                {
                    ProductId = selectedProduct.ProductId,
                    ProductName = txtProductName.Text.Trim(),
                    Price = decimal.Parse(txtPrice.Text.Trim()),
                    StockQuantity = int.Parse(txtStockQuantity.Text.Trim())
                };
                await _productRepository.UpdateProductAsync(_currentUser.UserId, product);
                MessageBox.Show("Product updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadProductsAsync();
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
                MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                    await _productRepository.DeleteProductAsync(_currentUser.UserId, selectedProduct.ProductId);
                    MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadProductsAsync();
                    await LoadProductLogsAsync();
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
                    MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadProductsAsync();
            await LoadProductLogsAsync();
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                txtProductName.Text = selectedProduct.ProductName;
                txtPrice.Text = selectedProduct.Price.ToString();
                txtStockQuantity.Text = selectedProduct.StockQuantity.ToString();
            }
        }

        private bool ValidateProductInputs(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(txtProductName.Text.Trim()))
                errorMessage = "Product name is required.";
            else if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price <= 0)
                errorMessage = "Price must be a positive number.";
            else if (!int.TryParse(txtStockQuantity.Text.Trim(), out int stockQuantity) || stockQuantity < 0)
                errorMessage = "Stock quantity must be a non-negative integer.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void ClearInputs()
        {
            txtProductName.Clear();
            txtPrice.Clear();
            txtStockQuantity.Clear();
            dgvProducts.ClearSelection();
        }
    }
}