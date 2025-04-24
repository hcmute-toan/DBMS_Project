using System;
using System.Windows.Forms;

namespace LaptopStoreApp.Forms
{
    public partial class MainForm : Form
    {
        private UserControl _currentControl; // Track the current UserControl (optional)

        public MainForm()
        {
            InitializeComponent();
        }

        // Helper method to load a UserControl into the panel
        private void LoadUserControl(UserControl userControl)
        {
            try
            {
                // Dispose of the previous UserControl (if any)
                if (_currentControl != null)
                {
                    _currentControl.Dispose();
                }

                // Clear the panel
                panel1.Controls.Clear();

                // Set the new UserControl
                _currentControl = userControl;
                _currentControl.Dock = DockStyle.Fill;
                panel1.Controls.Add(_currentControl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải giao diện: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Product_Click(object sender, EventArgs e)
        {
            LoadUserControl(new ProductForm());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadUserControl(new ImportForm());
        }

        private void Export_Click(object sender, EventArgs e)
        {
            LoadUserControl(new ExportForm());
        }

        private void Category_Click(object sender, EventArgs e)
        {
            LoadUserControl(new CategoryForm());
        }

        private void Supplier_Click(object sender, EventArgs e)
        {
            LoadUserControl(new SupplierForm());
        }

        private void Customer_Click(object sender, EventArgs e)
        {
            LoadUserControl(new CustomerForm());
        }

        private void Notification_Click(object sender, EventArgs e)
        {
            LoadUserControl(new NotificationForm());
        }

        private void Warehouse_Click(object sender, EventArgs e)
        {
            LoadUserControl(new WarehouseForm());
        }

        private void Spec_Click(object sender, EventArgs e)
        {
            LoadUserControl(new SpecForm());
        }

        private void Role_Click(object sender, EventArgs e)
        {
            LoadUserControl(new RoleForm());
        }

        // Optional: Clean up when the MainForm is closing
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_currentControl != null)
            {
                _currentControl.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}