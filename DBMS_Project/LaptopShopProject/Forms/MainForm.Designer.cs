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
            pictureBox1 = new PictureBox();
            btnUserManagement = new Button();
            btnProductManagement = new Button();
            btnImportManagement = new Button();
            btnExportManagement = new Button();
            btnSupplierManagement = new Button();
            btnCustomerManagement = new Button();
            btnCategoryManagement = new Button();
            btnReport = new Button();
            btnLogout = new Button();
            lblUserName = new Label();
            lbRoleUser = new Label();
            pnMainView = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = LaptopShopProject.Properties.Resources.MiddleGear_Logo;
            pictureBox1.Location = new Point(49, 5);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(197, 219);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // btnUserManagement
            // 
            btnUserManagement.Location = new Point(49, 260);
            btnUserManagement.Margin = new Padding(3, 4, 3, 4);
            btnUserManagement.Name = "btnUserManagement";
            btnUserManagement.Size = new Size(197, 44);
            btnUserManagement.TabIndex = 13;
            btnUserManagement.Text = "User Management";
            btnUserManagement.UseVisualStyleBackColor = true;
            btnUserManagement.Click += BtnUserManagement_Click;
            // 
            // btnProductManagement
            // 
            btnProductManagement.Location = new Point(49, 329);
            btnProductManagement.Margin = new Padding(3, 4, 3, 4);
            btnProductManagement.Name = "btnProductManagement";
            btnProductManagement.Size = new Size(197, 44);
            btnProductManagement.TabIndex = 14;
            btnProductManagement.Text = "Product Management";
            btnProductManagement.UseVisualStyleBackColor = true;
            btnProductManagement.Click += BtnProductManagement_Click;
            // 
            // btnImportManagement
            // 
            btnImportManagement.Location = new Point(49, 399);
            btnImportManagement.Margin = new Padding(3, 4, 3, 4);
            btnImportManagement.Name = "btnImportManagement";
            btnImportManagement.Size = new Size(197, 44);
            btnImportManagement.TabIndex = 15;
            btnImportManagement.Text = "Import Management";
            btnImportManagement.UseVisualStyleBackColor = true;
            btnImportManagement.Click += BtnImportManagement_Click;
            // 
            // btnExportManagement
            // 
            btnExportManagement.Location = new Point(49, 471);
            btnExportManagement.Margin = new Padding(3, 4, 3, 4);
            btnExportManagement.Name = "btnExportManagement";
            btnExportManagement.Size = new Size(197, 44);
            btnExportManagement.TabIndex = 16;
            btnExportManagement.Text = "Export Management";
            btnExportManagement.UseVisualStyleBackColor = true;
            btnExportManagement.Click += BtnExportManagement_Click;
            // 
            // btnSupplierManagement
            // 
            btnSupplierManagement.Location = new Point(49, 543);
            btnSupplierManagement.Margin = new Padding(3, 4, 3, 4);
            btnSupplierManagement.Name = "btnSupplierManagement";
            btnSupplierManagement.Size = new Size(197, 44);
            btnSupplierManagement.TabIndex = 17;
            btnSupplierManagement.Text = "Supplier Management";
            btnSupplierManagement.UseVisualStyleBackColor = true;
            btnSupplierManagement.Click += BtnSupplierManagement_Click;
            // 
            // btnCustomerManagement
            // 
            btnCustomerManagement.Location = new Point(49, 612);
            btnCustomerManagement.Margin = new Padding(3, 4, 3, 4);
            btnCustomerManagement.Name = "btnCustomerManagement";
            btnCustomerManagement.Size = new Size(197, 44);
            btnCustomerManagement.TabIndex = 18;
            btnCustomerManagement.Text = "Customer Management";
            btnCustomerManagement.UseVisualStyleBackColor = true;
            btnCustomerManagement.Click += BtnCustomerManagement_Click;
            // 
            // btnCategoryManagement
            // 
            btnCategoryManagement.Location = new Point(49, 683);
            btnCategoryManagement.Margin = new Padding(3, 4, 3, 4);
            btnCategoryManagement.Name = "btnCategoryManagement";
            btnCategoryManagement.Size = new Size(197, 44);
            btnCategoryManagement.TabIndex = 19;
            btnCategoryManagement.Text = "Category Management";
            btnCategoryManagement.UseVisualStyleBackColor = true;
            btnCategoryManagement.Click += BtnCategoryManagement_Click;
            // 
            // btnReport
            // 
            btnReport.Location = new Point(49, 751);
            btnReport.Margin = new Padding(3, 4, 3, 4);
            btnReport.Name = "btnReport";
            btnReport.Size = new Size(197, 44);
            btnReport.TabIndex = 20;
            btnReport.Text = "Report";
            btnReport.UseVisualStyleBackColor = true;
            btnReport.Click += BtnReport_Click;
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(49, 816);
            btnLogout.Margin = new Padding(3, 4, 3, 4);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(197, 44);
            btnLogout.TabIndex = 21;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += BtnLogout_Click;
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUserName.Location = new Point(273, 12);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(102, 23);
            lblUserName.TabIndex = 22;
            lblUserName.Text = "Username:";
            // 
            // lbRoleUser
            // 
            lbRoleUser.AutoSize = true;
            lbRoleUser.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbRoleUser.Location = new Point(273, 64);
            lbRoleUser.Name = "lbRoleUser";
            lbRoleUser.Size = new Size(56, 23);
            lbRoleUser.TabIndex = 23;
            lbRoleUser.Text = "Role:";
            // 
            // pnMainView
            // 
            pnMainView.Location = new Point(302, 93);
            pnMainView.Margin = new Padding(3, 4, 3, 4);
            pnMainView.Name = "pnMainView";
            pnMainView.Size = new Size(1058, 869);
            pnMainView.TabIndex = 24;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            ClientSize = new Size(1374, 1001);
            Controls.Add(pnMainView);
            Controls.Add(lbRoleUser);
            Controls.Add(lblUserName);
            Controls.Add(btnLogout);
            Controls.Add(btnReport);
            Controls.Add(btnCategoryManagement);
            Controls.Add(btnCustomerManagement);
            Controls.Add(btnSupplierManagement);
            Controls.Add(btnExportManagement);
            Controls.Add(btnImportManagement);
            Controls.Add(btnProductManagement);
            Controls.Add(btnUserManagement);
            Controls.Add(pictureBox1);
            Margin = new Padding(5, 4, 5, 4);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private PictureBox pictureBox1;
        private Button btnUserManagement;
        private Button btnProductManagement;
        private Button btnImportManagement;
        private Button btnExportManagement;
        private Button btnSupplierManagement;
        private Button btnCustomerManagement;
        private Button btnCategoryManagement;
        private Button btnReport;
        private Button btnLogout;
        private Label lblUserName;
        private Label lbRoleUser;
        private Panel pnMainView;
    }
}