using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class ImportManagementForm : UserControl
    {
        private readonly ImportRepository _importRepository;
        private readonly SupplierRepository _supplierRepository;
        private readonly User _currentUser;

        public ImportManagementForm(User currentUser)
        {
            InitializeComponent();
            _importRepository = new ImportRepository();
            _supplierRepository = new SupplierRepository();
            _currentUser = currentUser;
            LoadSuppliersAsync(); // Non-awaited in constructor
            LoadImportsAsync(); // Non-awaited in constructor
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvImports.SelectionChanged += dgvImports_SelectionChanged;
            dgvImportDetails.SelectionChanged += dgvImportDetails_SelectionChanged; // Thêm sự kiện cho Vấn đề 1
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var suppliers = await _supplierRepository.GetAllSuppliersAsync();
                cboSupplier.DataSource = suppliers;
                cboSupplier.DisplayMember = "SupplierName";
                cboSupplier.ValueMember = "SupplierId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadImportsAsync()
        {
            try
            {
                var imports = await _importRepository.GetAllImportsAsync();
                dgvImports.DataSource = null; // Clear existing data
                dgvImports.DataSource = imports;
                ConfigureImportDataGridView();
                if (imports.Any())
                {
                    await LoadImportDetailsAsync(imports.First().ImportId);
                }
                else
                {
                    dgvImportDetails.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading imports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadImportDetailsAsync(int importId)
        {
            try
            {
                var details = await _importRepository.GetImportDetailsAsync(importId);
                dgvImportDetails.DataSource = details;
                ConfigureImportDetailsDataGridView();
                decimal totalAmount = await _importRepository.GetImportTotalAsync(importId);
                lblTotalAmount.Text = $"Total Amount: {totalAmount:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading import details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureImportDataGridView()
        {
            if (dgvImports.Columns.Contains("ImportId"))
                dgvImports.Columns["ImportId"].Visible = false;
            if (dgvImports.Columns.Contains("SupplierId"))
                dgvImports.Columns["SupplierId"].Visible = false;
            if (dgvImports.Columns.Contains("SupplierName"))
                dgvImports.Columns["SupplierName"].HeaderText = "Supplier";
            if (dgvImports.Columns.Contains("ImportDate"))
                dgvImports.Columns["ImportDate"].HeaderText = "Date";
            if (dgvImports.Columns.Contains("TotalAmount"))
            {
                dgvImports.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgvImports.Columns["TotalAmount"].DefaultCellStyle.Format = "C"; // Format as currency
            }
            dgvImports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigureImportDetailsDataGridView()
        {
            if (dgvImportDetails.Columns.Contains("ImportId"))
                dgvImportDetails.Columns["ImportId"].Visible = false;
            if (dgvImportDetails.Columns.Contains("ProductId"))
                dgvImportDetails.Columns["ProductId"].Visible = false;
            if (dgvImportDetails.Columns.Contains("ProductName"))
                dgvImportDetails.Columns["ProductName"].HeaderText = "Product";
            if (dgvImportDetails.Columns.Contains("Quantity"))
                dgvImportDetails.Columns["Quantity"].HeaderText = "Quantity";
            if (dgvImportDetails.Columns.Contains("UnitPrice"))
            {
                dgvImportDetails.Columns["UnitPrice"].HeaderText = "Unit Price";
                dgvImportDetails.Columns["UnitPrice"].DefaultCellStyle.Format = "C";
            }
            dgvImportDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateImportInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Thêm kiểm tra dữ liệu chi tiết nhập hàng
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Please enter a product name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Please enter a valid unit price greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal? price = null;
            if (!string.IsNullOrWhiteSpace(txtPrice.Text) && decimal.TryParse(txtPrice.Text, out decimal parsedPrice) && parsedPrice > 0)
            {
                price = parsedPrice;
            }

            try
            {
                int supplierId = (int)cboSupplier.SelectedValue;
                DateTime importDate = dtpImportDate.Value.Date;

                // Sửa logic kiểm tra phiếu nhập: Chỉ kiểm tra SupplierId, lấy phiếu nhập mới nhất
                var existingImport = (await _importRepository.GetAllImportsAsync())
                    .Where(i => i.SupplierId == supplierId)
                    .OrderByDescending(i => i.ImportId) // Lấy phiếu nhập mới nhất
                    .FirstOrDefault();

                int importId;
                if (existingImport != null)
                {
                    // Sử dụng phiếu nhập mới nhất từ nhà cung cấp
                    importId = existingImport.ImportId;
                    MessageBox.Show($"Using existing import ID: {importId} for supplier {cboSupplier.Text}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Tạo phiếu nhập mới
                    var import = new Import
                    {
                        SupplierId = supplierId,
                        ImportDate = importDate,
                        TotalAmount = 0
                    };
                    importId = await _importRepository.InsertImportAsync(_currentUser.UserId, import);
                    MessageBox.Show($"Import added successfully with ID: {importId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Thêm chi tiết nhập hàng
                var detail = new ImportDetail
                {
                    ImportId = importId,
                    ProductName = txtProductName.Text,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };
                await _importRepository.InsertImportDetailAsync(_currentUser.UserId, detail, price);

                ClearInputs();
                await LoadImportsAsync();
                // Tự động chọn lại phiếu nhập vừa thêm/sử dụng để hiển thị chi tiết
                foreach (DataGridViewRow row in dgvImports.Rows)
                {
                    var import = (Import)row.DataBoundItem;
                    if (import.ImportId == importId)
                    {
                        row.Selected = true;
                        await LoadImportDetailsAsync(importId);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an import to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvImportDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an import detail to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateImportInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                var selectedDetail = (ImportDetail)dgvImportDetails.SelectedRows[0].DataBoundItem;

                if (!int.TryParse(txtQuantity.Text, out int newQuantity))
                {
                    MessageBox.Show("Please enter a valid quantity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtUnitPrice.Text, out decimal newUnitPrice))
                {
                    MessageBox.Show("Please enter a valid unit price.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal newPrice;
                if (!string.IsNullOrWhiteSpace(txtPrice.Text) && decimal.TryParse(txtPrice.Text, out decimal parsedPrice))
                {
                    newPrice = parsedPrice;
                }
                else
                {
                    // Nếu không nhập giá mới, lấy giá hiện tại từ sản phẩm
                    var product = await GetProductByNameAsync(selectedDetail.ProductName);
                    newPrice = product?.Price ?? selectedDetail.UnitPrice;
                }

                // Cập nhật ImportDetail và Price
                var updatedDetail = new ImportDetail
                {
                    ImportId = selectedImport.ImportId,
                    ProductName = selectedDetail.ProductName,
                    Quantity = newQuantity,
                    UnitPrice = newUnitPrice
                };
                bool isUpdated = await _importRepository.UpdateImportAsync(_currentUser.UserId, updatedDetail, newPrice);

                // Hiển thị thông báo dựa trên kết quả từ stored procedure
                if (isUpdated)
                {
                    MessageBox.Show("Import updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No changes detected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Làm mới giao diện
                await LoadImportsAsync();

                // Tự động chọn lại dòng vừa cập nhật
                foreach (DataGridViewRow row in dgvImports.Rows)
                {
                    var currentImport = (Import)row.DataBoundItem;
                    if (currentImport.ImportId == selectedImport.ImportId)
                    {
                        row.Selected = true;
                        await LoadImportDetailsAsync(selectedImport.ImportId);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an import to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this import?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                    await _importRepository.DeleteImportAsync(_currentUser.UserId, selectedImport.ImportId);
                    MessageBox.Show("Import deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadImportsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadImportsAsync();
        }

        private void dgvImports_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImports.SelectedRows.Count > 0)
            {
                var selectedImport = (Import)dgvImports.SelectedRows[0].DataBoundItem;
                cboSupplier.SelectedValue = selectedImport.SupplierId;
                dtpImportDate.Value = selectedImport.ImportDate;
                LoadImportDetailsAsync(selectedImport.ImportId);
                ClearDetailInputs(); // Xóa các ô nhập liệu chi tiết khi chọn phiếu nhập khác
            }
        }

        private async void dgvImportDetails_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImportDetails.SelectedRows.Count > 0)
            {
                var selectedDetail = (ImportDetail)dgvImportDetails.SelectedRows[0].DataBoundItem;
                txtProductName.Text = selectedDetail.ProductName;
                txtQuantity.Text = selectedDetail.Quantity.ToString();
                txtUnitPrice.Text = selectedDetail.UnitPrice.ToString("F2");
                // txtPrice không có thông tin từ ImportDetail, để trống hoặc lấy từ Product nếu cần
                var product = await GetProductByNameAsync(selectedDetail.ProductName);
                txtPrice.Text = product != null ? product.Price.ToString("F2") : string.Empty;
            }
            else
            {
                ClearDetailInputs();
            }
        }

        private async Task<Product> GetProductByNameAsync(string productName)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT product_id, product_name, price, stock_quantity FROM Product WHERE product_name = @productName", conn))
                    {
                        cmd.Parameters.AddWithValue("@productName", productName);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    StockQuantity = reader.GetInt32(3)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private bool ValidateImportInputs(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (cboSupplier.SelectedValue == null)
                errorMessage = "Please select a supplier.";
            else if (dtpImportDate.Value > DateTime.Now)
                errorMessage = "Import date cannot be in the future.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void ClearInputs()
        {
            cboSupplier.SelectedIndex = -1;
            dtpImportDate.Value = DateTime.Now;
            dgvImports.ClearSelection();
            dgvImportDetails.DataSource = null;
            lblTotalAmount.Text = "Total Amount: 0";
            ClearDetailInputs();
        }

        private void ClearDetailInputs()
        {
            txtProductName.Clear();
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            txtPrice.Clear();
            dgvImportDetails.ClearSelection();
        }
    }

}
