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
        private readonly string _username;
        private readonly string _role;
        private readonly string _password;

        public ProductManagementForm(string username, string role, string password)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            _password =password;
            _productRepository = new ProductRepository(_username, _password);
            ConfigurePermissions();
            InitializeSearchComboBox();
            LoadProductsAsync();
            LoadProductLogsAsync();
            ConfigureEventHandlers();
        }

        private void ConfigurePermissions()
        {
            if (_role.Equals("employee_role", StringComparison.OrdinalIgnoreCase))
            {
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void InitializeSearchComboBox()
        {
            cbChooseSearchByNameOrCategory.Items.AddRange(new object[] { "Name", "Category" });
            cbChooseSearchByNameOrCategory.SelectedIndex = 0;
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;
            searchByNameOrCategory.Click += searchByNameOrCategory_Click;
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                dgvProducts.DataSource = products;
                ConfigureProductDataGridView();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false;
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
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvProductLogs.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureProductDataGridView()
        {
            if (dgvProducts.Columns.Contains("ProductId"))
            {
                dgvProducts.Columns["ProductId"].HeaderText = "ID";
                dgvProducts.Columns["ProductId"].Visible = false;
            }
            if (dgvProducts.Columns.Contains("ProductName"))
                dgvProducts.Columns["ProductName"].HeaderText = "Name";
            if (dgvProducts.Columns.Contains("Price"))
            {
                dgvProducts.Columns["Price"].HeaderText = "Price";
                dgvProducts.Columns["Price"].DefaultCellStyle.Format = "N0";
            }
            if (dgvProducts.Columns.Contains("StockQuantity"))
                dgvProducts.Columns["StockQuantity"].HeaderText = "Stock Quantity";
            if (dgvProducts.Columns.Contains("Brands"))
                dgvProducts.Columns["Brands"].HeaderText = "Brands";
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureProductLogsDataGridView()
        {
            if (dgvProductLogs.Columns.Contains("LogId"))
            {
                dgvProductLogs.Columns["LogId"].HeaderText = "Log ID";
                dgvProductLogs.Columns["LogId"].Visible = false;
            }
            if (dgvProductLogs.Columns.Contains("ProductId"))
                dgvProductLogs.Columns["ProductId"].HeaderText = "Product ID";
            if (dgvProductLogs.Columns.Contains("ProductName"))
                dgvProductLogs.Columns["ProductName"].HeaderText = "Product Name";
            if (dgvProductLogs.Columns.Contains("Price"))
            {
                dgvProductLogs.Columns["Price"].HeaderText = "Price";
                dgvProductLogs.Columns["Price"].DefaultCellStyle.Format = "N0";
            }
            if (dgvProductLogs.Columns.Contains("StockQuantity"))
                dgvProductLogs.Columns["StockQuantity"].HeaderText = "Stock Quantity";
            if (dgvProductLogs.Columns.Contains("DeletedDate"))
                dgvProductLogs.Columns["DeletedDate"].HeaderText = "Deleted Date";
            if (dgvProductLogs.Columns.Contains("DeletedBy"))
            {
                dgvProductLogs.Columns["DeletedBy"].HeaderText = "Deleted By";
                dgvProductLogs.Columns["DeletedBy"].Visible = false;
            }
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
                    StockQuantity = int.Parse(txtStockQuantity.Text.Trim()),
                    Brands = txtBranch.Text.Trim() != "" ? txtBranch.Text.Trim() : null
                };
                int productId = await _productRepository.InsertProductAsync(product);
                MessageBox.Show($"Product added successfully with ID: {productId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadProductsAsync();
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    var productRow = (Product)row.DataBoundItem;
                    if (productRow.ProductId == productId)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "adding product");
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
                    StockQuantity = int.Parse(txtStockQuantity.Text.Trim()),
                    Brands = txtBranch.Text.Trim() != "" ? txtBranch.Text.Trim() : null
                };
                await _productRepository.UpdateProductAsync(product);
                MessageBox.Show("Product updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadProductsAsync();
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    var productRow = (Product)row.DataBoundItem;
                    if (productRow.ProductId == selectedProduct.ProductId)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "updating product");
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
                    await _productRepository.DeleteProductAsync(selectedProduct.ProductId);
                    MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadProductsAsync();
                    await LoadProductLogsAsync();
                }
                catch (Exception ex)
                {
                    HandleException(ex, "deleting product");
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            txtNameOrCategory.Clear();
            cbChooseSearchByNameOrCategory.SelectedIndex = 0;
            await LoadProductsAsync();
            await LoadProductLogsAsync();
        }

        private async void searchByNameOrCategory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNameOrCategory.Text))
            {
                await LoadProductsAsync();
                return;
            }

            try
            {
                var searchText = txtNameOrCategory.Text.Trim();
                var searchType = cbChooseSearchByNameOrCategory.SelectedItem?.ToString();

                List<Product> products = null;
                if (searchType == "Name")
                {
                    products = await _productRepository.SearchProductByNameAsync(searchText);
                }
                else if (searchType == "Category")
                {
                    products = await _productRepository.SearchProductByCategoryAsync(searchText);
                }
                else
                {
                    MessageBox.Show("Please select a valid search type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dgvProducts.DataSource = products;
                ConfigureProductDataGridView();
                if (!products.Any())
                {
                    MessageBox.Show("No products found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "searching products");
            }
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                txtProductName.Text = selectedProduct.ProductName;
                txtPrice.Text = selectedProduct.Price.ToString("N0");
                txtStockQuantity.Text = selectedProduct.StockQuantity.ToString();
                txtBranch.Text = selectedProduct.Brands ?? string.Empty;
            }
            else
            {
                ClearInputs();
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
            txtBranch.Clear();
            dgvProducts.ClearSelection();
        }

        private void HandleException(Exception ex, string action)
        {
            string message;
            string title;
            MessageBoxIcon icon;

            switch (ex)
            {
                case InvalidOperationException:
                    message = ex.Message;
                    title = "Operation Failed";
                    icon = MessageBoxIcon.Warning;
                    break;
                case UnauthorizedAccessException:
                    message = ex.Message;
                    title = "Permission Denied";
                    icon = MessageBoxIcon.Error;
                    break;
                case KeyNotFoundException:
                    message = ex.Message;
                    title = "Not Found";
                    icon = MessageBoxIcon.Warning;
                    break;
                default:
                    message = $"Error {action}: {ex.Message}";
                    title = "Error";
                    icon = MessageBoxIcon.Error;
                    break;
            }

            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }
}