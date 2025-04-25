using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LaptopShopProject.Forms
{
    public partial class ExportManagementForm : Form
    {
        private readonly ExportRepository _exportRepository;
        private readonly ProductRepository _productRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly User _currentUser;
        private ComboBox cboProduct; // Added ComboBox for selecting products

        public ExportManagementForm(User currentUser)
        {
            InitializeComponent();
            _exportRepository = new ExportRepository();
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository();
            _currentUser = currentUser;

            // Initialize cboProduct (not in designer, adding programmatically)
            cboProduct = new ComboBox
            {
                Location = new System.Drawing.Point(46, 76),
                Size = new System.Drawing.Size(249, 28),
                Name = "cboProduct",
                FormattingEnabled = true
            };
            cboProduct.SelectedIndexChanged += cboProduct_SelectedIndexChanged;
            this.Controls.Add(cboProduct);

            LoadCustomers();
            LoadProducts();
            LoadExports();
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerRepository.GetAllCustomers(_currentUser.UserId);
                cboCustomer.DataSource = customers;
                cboCustomer.DisplayMember = "CustomerName";
                cboCustomer.ValueMember = "CustomerId";
                cboCustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khách hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productRepository.GetAllProducts(_currentUser.UserId);
                cboProduct.DataSource = products;
                cboProduct.DisplayMember = "ProductName";
                cboProduct.ValueMember = "ProductId";
                cboProduct.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExports()
        {
            try
            {
                var exports = _exportRepository.GetAllExports();
                dgvExports.DataSource = exports;
                dgvExports.Columns["ExportId"].HeaderText = "Mã Xuất";
                dgvExports.Columns["CustomerId"].Visible = false;
                dgvExports.Columns["CustomerName"].HeaderText = "Khách Hàng";
                dgvExports.Columns["ExportDate"].HeaderText = "Ngày Xuất";
                dgvExports.Columns["TotalAmount"].HeaderText = "Tổng Tiền";

                if (exports.Count > 0)
                {
                    dgvExports.Rows[0].Selected = true;
                    LoadExportDetails(exports[0].ExportId);
                }
                else
                {
                    dgvExportDetails.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách phiếu xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExportDetails(int exportId)
        {
            try
            {
                var details = _exportRepository.GetExportDetails(exportId);
                dgvExportDetails.DataSource = details;
                dgvExportDetails.Columns["ExportId"].Visible = false;
                dgvExportDetails.Columns["ProductId"].Visible = false;
                dgvExportDetails.Columns["ProductName"].HeaderText = "Tên Sản Phẩm";
                dgvExportDetails.Columns["Quantity"].HeaderText = "Số Lượng";
                dgvExportDetails.Columns["UnitPrice"].HeaderText = "Đơn Giá";

                decimal totalAmount = _exportRepository.GetExportTotal(exportId);
                lblTotalAmount.Text = $"Tổng Tiền: {totalAmount:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chi tiết phiếu xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                // Create new Export
                var export = new Export
                {
                    CustomerId = (int)cboCustomer.SelectedValue,
                    ExportDate = dtpExportDate.Value,
                    TotalAmount = 0 // Will be updated after adding details
                };

                int exportId = _exportRepository.InsertExport(_currentUser.UserId, export);

                // Add Export Detail
                var detail = new ExportDetail
                {
                    ExportId = exportId,
                    ProductId = (int)cboProduct.SelectedValue,
                    Quantity = int.Parse(txtQuantity.Text),
                    UnitPrice = decimal.Parse(txtUnitPrice.Text)
                };

                _exportRepository.InsertExportDetail(_currentUser.UserId, detail);

                // Update Total Amount
                decimal totalAmount = _exportRepository.GetExportTotal(exportId);
                export.ExportId = exportId;
                export.TotalAmount = totalAmount;
                _exportRepository.UpdateExport(_currentUser.UserId, export);

                MessageBox.Show("Thêm phiếu xuất thành công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadExports();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phiếu xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvExports.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn phiếu xuất cần cập nhật!", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput()) return;

                var selectedExport = (Export)dgvExports.SelectedRows[0].DataBoundItem;
                var export = new Export
                {
                    ExportId = selectedExport.ExportId,
                    CustomerId = (int)cboCustomer.SelectedValue,
                    ExportDate = dtpExportDate.Value,
                    TotalAmount = _exportRepository.GetExportTotal(selectedExport.ExportId)
                };

                _exportRepository.UpdateExport(_currentUser.UserId, export);
                MessageBox.Show("Cập nhật phiếu xuất thành công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadExports();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật phiếu xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvExports.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn phiếu xuất cần xóa!", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedExport = (Export)dgvExports.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show($"Bạn có chắc muốn xóa phiếu xuất {selectedExport.ExportId}?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _exportRepository.DeleteExport(_currentUser.UserId, selectedExport.ExportId);
                    MessageBox.Show("Xóa phiếu xuất thành công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadExports();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa phiếu xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadExports();
            ClearInputs();
        }

        private void dgvExports_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvExports.SelectedRows.Count > 0)
            {
                var selectedExport = (Export)dgvExports.SelectedRows[0].DataBoundItem;
                cboCustomer.SelectedValue = selectedExport.CustomerId;
                dtpExportDate.Value = selectedExport.ExportDate;
                LoadExportDetails(selectedExport.ExportId);
            }
        }

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle customer selection if needed
        }

        private void cboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProduct.SelectedItem != null)
            {
                var selectedProduct = (Product)cboProduct.SelectedItem;
                txtUnitPrice.Text = selectedProduct.Price.ToString("F2");
            }
        }

        private bool ValidateInput()
        {
            if (cboCustomer.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboProduct.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ!", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var selectedProduct = (Product)cboProduct.SelectedItem;
            int stock = _productRepository.GetStockQuantity(selectedProduct.ProductId);
            if (quantity > stock)
            {
                MessageBox.Show($"Số lượng xuất vượt quá tồn kho! Tồn kho hiện tại: {stock}", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ!", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            cboCustomer.SelectedIndex = -1;
            cboProduct.SelectedIndex = -1;
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            dtpExportDate.Value = DateTime.Now;
        }
    }
}