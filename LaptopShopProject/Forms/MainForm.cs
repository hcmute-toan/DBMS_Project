using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopStoreApp.Forms
{
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Product_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();

            ProductForm productForm = new ProductForm();

            productForm.Dock = DockStyle.Fill;  

            panel1.Controls.Add(productForm);

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();

            ImportForm importForm = new ImportForm();

            importForm.Dock = DockStyle.Fill;

            panel1.Controls.Add(importForm);
        }

        private void Export_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            ExportForm exportForm = new ExportForm();
            exportForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(exportForm);
        }

        private void Category_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            CategoryForm categoryForm = new CategoryForm();
            categoryForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(categoryForm);
        }

        private void Supplier_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            SupplierForm supplierForm = new SupplierForm();
            supplierForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(supplierForm);
        }

        private void Customer_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            CustomerForm customerForm = new CustomerForm();
            customerForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(customerForm);
        }

        private void Notification_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            NotificationForm notificationForm = new NotificationForm();
            notificationForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(notificationForm);
        }

        private void Warehouse_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            WarehouseForm warehouseForm = new WarehouseForm();
            warehouseForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(warehouseForm);
        }

        private void Spec_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            SpecForm specForm = new SpecForm();
            specForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(specForm);
        }

        private void Role_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            RoleForm roleForm = new RoleForm();
            roleForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(roleForm);
        }
    }
}
