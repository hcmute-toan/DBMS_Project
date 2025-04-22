namespace LaptopStoreApp.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Product = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Export = new System.Windows.Forms.Button();
            this.Category = new System.Windows.Forms.Button();
            this.Supplier = new System.Windows.Forms.Button();
            this.Customer = new System.Windows.Forms.Button();
            this.Notification = new System.Windows.Forms.Button();
            this.Warehouse = new System.Windows.Forms.Button();
            this.Spec = new System.Windows.Forms.Button();
            this.Role = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Product
            // 
            this.Product.Location = new System.Drawing.Point(26, 25);
            this.Product.Name = "Product";
            this.Product.Size = new System.Drawing.Size(252, 31);
            this.Product.TabIndex = 1;
            this.Product.Text = "Products";
            this.Product.UseVisualStyleBackColor = true;
            this.Product.Click += new System.EventHandler(this.Product_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(26, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(252, 31);
            this.button2.TabIndex = 2;
            this.button2.Text = "Imports";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Export
            // 
            this.Export.Location = new System.Drawing.Point(26, 131);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(252, 31);
            this.Export.TabIndex = 4;
            this.Export.Text = "Exports";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // Category
            // 
            this.Category.Location = new System.Drawing.Point(26, 189);
            this.Category.Name = "Category";
            this.Category.Size = new System.Drawing.Size(252, 31);
            this.Category.TabIndex = 5;
            this.Category.Text = "Categories";
            this.Category.UseVisualStyleBackColor = true;
            this.Category.Click += new System.EventHandler(this.Category_Click);
            // 
            // Supplier
            // 
            this.Supplier.Location = new System.Drawing.Point(26, 243);
            this.Supplier.Name = "Supplier";
            this.Supplier.Size = new System.Drawing.Size(252, 31);
            this.Supplier.TabIndex = 6;
            this.Supplier.Text = "Suppliers";
            this.Supplier.UseVisualStyleBackColor = true;
            this.Supplier.Click += new System.EventHandler(this.Supplier_Click);
            // 
            // Customer
            // 
            this.Customer.Location = new System.Drawing.Point(26, 298);
            this.Customer.Name = "Customer";
            this.Customer.Size = new System.Drawing.Size(252, 31);
            this.Customer.TabIndex = 7;
            this.Customer.Text = "Customers";
            this.Customer.UseVisualStyleBackColor = true;
            this.Customer.Click += new System.EventHandler(this.Customer_Click);
            // 
            // Notification
            // 
            this.Notification.Location = new System.Drawing.Point(26, 356);
            this.Notification.Name = "Notification";
            this.Notification.Size = new System.Drawing.Size(252, 31);
            this.Notification.TabIndex = 8;
            this.Notification.Text = "Notifications";
            this.Notification.UseVisualStyleBackColor = true;
            this.Notification.Click += new System.EventHandler(this.Notification_Click);
            // 
            // Warehouse
            // 
            this.Warehouse.Location = new System.Drawing.Point(26, 419);
            this.Warehouse.Name = "Warehouse";
            this.Warehouse.Size = new System.Drawing.Size(252, 31);
            this.Warehouse.TabIndex = 9;
            this.Warehouse.Text = "Warehouses";
            this.Warehouse.UseVisualStyleBackColor = true;
            this.Warehouse.Click += new System.EventHandler(this.Warehouse_Click);
            // 
            // Spec
            // 
            this.Spec.Location = new System.Drawing.Point(26, 479);
            this.Spec.Name = "Spec";
            this.Spec.Size = new System.Drawing.Size(252, 31);
            this.Spec.TabIndex = 10;
            this.Spec.Text = "Specs";
            this.Spec.UseVisualStyleBackColor = true;
            this.Spec.Click += new System.EventHandler(this.Spec_Click);
            // 
            // Role
            // 
            this.Role.Location = new System.Drawing.Point(26, 531);
            this.Role.Name = "Role";
            this.Role.Size = new System.Drawing.Size(252, 31);
            this.Role.TabIndex = 11;
            this.Role.Text = "Roles";
            this.Role.UseVisualStyleBackColor = true;
            this.Role.Click += new System.EventHandler(this.Role_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(333, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(961, 537);
            this.panel1.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1306, 591);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Role);
            this.Controls.Add(this.Spec);
            this.Controls.Add(this.Warehouse);
            this.Controls.Add(this.Notification);
            this.Controls.Add(this.Customer);
            this.Controls.Add(this.Supplier);
            this.Controls.Add(this.Category);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Product);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Product;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.Button Category;
        private System.Windows.Forms.Button Supplier;
        private System.Windows.Forms.Button Customer;
        private System.Windows.Forms.Button Notification;
        private System.Windows.Forms.Button Warehouse;
        private System.Windows.Forms.Button Spec;
        private System.Windows.Forms.Button Role;
        private System.Windows.Forms.Panel panel1;
    }
}